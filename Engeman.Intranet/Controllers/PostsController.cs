using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Engeman.Intranet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Extensions;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class PostsController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPostFileRepository _postFileRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentFileRepository _commentFileRepository;
    private readonly IPostRestrictionRepository _postRestrictionRepository;
    private readonly IConfiguration _configuration;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository,
      IDepartmentRepository departmentRepository, IPostFileRepository postFileRepository, ICommentRepository postCommentRepository,
      ICommentFileRepository postCommentFileRepository, IPostRestrictionRepository postRestrictionRepository, IConfiguration configuration)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _postFileRepository = postFileRepository;
      _commentRepository = postCommentRepository;
      _commentFileRepository = postCommentFileRepository;
      _postRestrictionRepository = postRestrictionRepository;
      _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Grid()
    {
      if (Request.Query["filter"] != "allPosts" && HttpContext.Session.Get<bool>("_IsModerator") == false) return Redirect(Request.Host.ToString());

      ViewBag.FilterGrid = Request.Query["filter"];
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView("PostsGrid");
    }

    [HttpPost]
    public JsonResult DataGrid(string filterGrid, string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<PostGridViewModel> posts = null;
      IQueryable paginatedPosts;
      var total = 0;
      var isModerator = CheckIsModerator();
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      string order;
      string field;
      string orderedField = string.Empty;

      if (!string.IsNullOrEmpty(key))
      {
        order = requestKeys[key];
        field = key.Replace("sort[", "").Replace("]", "");
        orderedField = string.Format("{0} {1}", field, order);
      }

      posts = FilterPosts(filterGrid, filterHeader, searchPhrase);
      total = posts.Count();

      if (rowCount == -1) rowCount = total;

      paginatedPosts = OrderedPosts(posts, orderedField, current, rowCount);

      return Json(new { rows = paginatedPosts, current, rowCount, total, isModerator });
    }

    public IQueryable<PostGridViewModel> FilterPosts(string filterGrid, string filterHeader, string searchPhrase)
    {
      var user = new UserAccount();
      IQueryable<PostGridViewModel> posts = null;
      var comments = new List<Comment>();

      if (bool.Parse(_configuration.GetSection("SEARCH_CONDITION:CONTAINSTABLE").Value) == true && (!string.IsNullOrEmpty(searchPhrase)))
      {
        searchPhrase = Regex.Replace(searchPhrase, @"^[\s]+|[\s]+$", "");
        searchPhrase = Regex.Replace(searchPhrase, @"\s+", " NEAR ");
      }

      try
      {
        user = _userAccountRepository.GetById(HttpContext.Session.Get<int>("_CurrentUserId"));
        posts = _postRepository.GetPostsGrid(user, searchPhrase).AsQueryable();
      }
      catch (Exception) { }

      foreach (var post in posts)
      {
        try { comments = _commentRepository.GetByPostId(post.Id); } catch (Exception) { }
        for (int i = 0; i < comments.Count; i++)
        {
          if (comments[i].Revised == false)
          {
            post.UnrevisedComments = true;
            break;
          }
        }
      }

      if (filterGrid == "allPosts")
      {
        if (user.Moderator == true) posts = posts.Where("revised == (@0) && UnrevisedComments == (@1) ", true, false);
      }
      else if (filterGrid == "unrevisedPosts") posts = posts.Where("revised == (@0)", false);
      else if (filterGrid == "unrevisedComments")
      {
        try { posts = _postRepository.GetWithUnrevisedComments().AsQueryable(); } catch (Exception) { }
      }
      if (filterHeader == "manual") return posts = posts.Where("postType == (@0)", "M");
      else if (filterHeader == "document") return posts = posts.Where("postType == (@0)", "D");
      else if (filterHeader == "informative") return posts = posts.Where("postType == (@0)", "I");
      else if (filterHeader == "question") return posts = posts.Where("postType == (@0)", "Q");
      else if (filterHeader == "my") return posts = posts.Where(x => x.UserAccountId == user.Id);

      else return posts;
    }

    public IQueryable OrderedPosts(IQueryable<PostGridViewModel> posts, string orderedField, int current, int rowCount)
    {
      if (orderedField.Contains("changeDate asc")) return posts.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      else if (orderedField.Contains("changeDate desc")) return posts.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      else if (!string.IsNullOrEmpty(orderedField)) return posts.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
      else return posts.Skip((current - 1) * rowCount).Take(rowCount);
    }

    [HttpGet]
    public IActionResult NewPost()
    {
      var permissions = new UserPermissionsViewModel();

      try { permissions = _userAccountRepository.GetUserPermissionsByUsername(HttpContext.Session.Get<string>("_CurrentUsername")); } catch (Exception) { }

      if (permissions.CreatePost == true)
      {
        ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");
        try { ViewBag.Departments = _departmentRepository.Get(); }
        catch (Exception) { }
        return PartialView("NewPost");
      }
      else return StatusCode(StatusCodes.Status401Unauthorized);
    }

    [HttpPost]
    public IActionResult NewPost(NewPostViewModel newPost, List<IFormFile> addFiles)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      var userAccount = new UserAccount();

      try { userAccount = _userAccountRepository.GetByUsername(sessionUsername); } catch (Exception) { }

      if (userAccount.Moderator == true || userAccount.NoviceUser == false) newPost.Revised = true;

      newPost.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(newPost.Description));

      newPost.UserAccountId = userAccount.Id;

      if (newPost.Keywords != null) newPost.Keywords = newPost.Keywords.ToLower();

      if (addFiles.Count > 0)
      {
        for (int i = 0; i < addFiles.Count; i++)
        {
          var file = new NewPostFileViewModel();
          file.Name = addFiles[i].FileName;
          if (addFiles[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              addFiles[i].CopyTo(stream);
              file.BinaryData = stream.ToArray();
            }
          }
          newPost.Files.Add(file);
        }
      }

      try { _postRepository.Add(newPost, sessionUsername); }
      catch (SqlException ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult EditPost(int postId)
    {
      if (!HttpContext.Request.IsAjax("GET")) return Redirect(Request.Host.ToString());

      var restrictedDepartments = new List<int>();
      var postEditViewModel = new PostEditViewModel();
      var orderedFiles = new List<PostFile>();
      var departments = new List<Department>();
      var postTypes = new Dictionary<string, char>();
      var post = new Post();

      try
      {
        orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList();
        post = _postRepository.GetById(postId);
        departments = _departmentRepository.Get();
      }
      catch (Exception) { }

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");
      ViewBag.Departments = departments;
      postEditViewModel.Id = post.Id;
      postEditViewModel.Restricted = post.Restricted;
      postEditViewModel.Subject = post.Subject;
      postEditViewModel.Description = post.Description;
      postEditViewModel.Keywords = post.Keywords;
      postEditViewModel.Revised = post.Revised;
      postEditViewModel.PostType = post.PostType;

      if (orderedFiles.Count != 0)
      {
        for (int i = 0; i < orderedFiles.Count; i++)
        {
          postEditViewModel.Files.Add(orderedFiles[i]);
        }
      }
      if (post.Restricted == true)
      {
        try { restrictedDepartments = _postRestrictionRepository.GetDepartmentsByIdPost(postId); } catch (Exception) { }

        ViewBag.RestrictedDepartments = restrictedDepartments;
      }

      ViewBag.PostTypes = postEditViewModel.PostTypeDictionary;

      return PartialView(postEditViewModel);
    }

    [HttpPut]
    public IActionResult UpdatePost(PostEditViewModel editedPost, List<IFormFile> addFiles)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      var fileList = new List<NewPostFileViewModel>();
      var currentPost = new Post();
      var userAccount = new UserAccount();
      int[] filesToBeRemove = null;

      if (Request.Cookies["FilesToBeRemove"] != string.Empty)
      {
        filesToBeRemove = (Regex.Replace(Request.Cookies["FilesToBeRemove"], @"(^\s*)|(\s*$)", "")).Split(' ').Select(int.Parse).ToArray();
      }

      try
      {
        currentPost = _postRepository.GetById(editedPost.Id);
        userAccount = _userAccountRepository.GetByUsername(sessionUsername);
      }
      catch (Exception) { }

      editedPost.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(editedPost.Description));

      if (editedPost.Keywords != null) editedPost.Keywords = editedPost.Keywords.ToLower();

      if (addFiles.Count != 0)
      {
        fileList.Clear();
        for (int i = 0; i < addFiles.Count; i++)
        {
          NewPostFileViewModel newFile = new NewPostFileViewModel();
          if (addFiles[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              addFiles[i].CopyTo(stream);
              newFile.BinaryData = stream.ToArray();
              newFile.Name = addFiles[i].FileName;
              fileList.Add(newFile);
            }
          }
        }
      }

      if (currentPost.Revised == true && userAccount.NoviceUser == false) editedPost.Revised = true;

      try
      {
        _postRepository.Update(editedPost.Id, editedPost, sessionUsername);
        if (filesToBeRemove != null) _postFileRepository.Delete(filesToBeRemove);
        _postFileRepository.Add(editedPost.Id, fileList);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }

      ViewBag.FilterGrid = Request.Query["filter"];

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public IActionResult DeletePost(int postId)
    {
      if (!HttpContext.Request.IsAjax("DELETE")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _postRepository.Delete(postId, currentUsername); } catch (Exception) { }

      return PartialView("PostsGrid");
    }

    [HttpGet]
    public IActionResult PostDetails(int postId)
    {
      if (!HttpContext.Request.IsAjax("GET")) return Redirect(Request.Host.ToString());

      var postDetails = new PostDetailsViewModel();
      var commentFiles = new List<CommentFile>();
      var comments = new List<CommentViewModel>();
      var post = new Post();
      var postAuthor = new UserAccount();
      var orderedFiles = new List<PostFile>();
      var department = new Department();
      var postsCount = 0;
      var commentsCount = 0;
      var userAccount = new UserAccount();
      var orderedComments = new List<Comment>();
      string[] keywords;

      try
      {
        post = _postRepository.GetById(postId);
        postAuthor = _userAccountRepository.GetById(post.UserAccountId);
        orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList();
        department = _departmentRepository.GetById(postAuthor.DepartmentId);
        postsCount = _postRepository.CountByUserId(postAuthor.Id);
        commentsCount = _commentRepository.CountByUserId(postAuthor.Id);
        userAccount = _userAccountRepository.GetById(HttpContext.Session.Get<int>("_CurrentUserId"));
        orderedComments = _commentRepository.GetByUserRestriction(userAccount, postId);
      }
      catch (Exception) { }

      postDetails.Id = post.Id;
      postDetails.Subject = post.Subject;
      postDetails.Description = post.Description;
      postDetails.Files = orderedFiles;
      postDetails.AuthorId = postAuthor.Id;
      postDetails.AuthorUsername = postAuthor.Name;
      postDetails.AuthorDepartment = department.Description;
      postDetails.AuthorPostsMade = postsCount;
      postDetails.AuthorCommentsMade = commentsCount;
      postDetails.AuthorPhoto = postAuthor.Photo;
      postDetails.Revised = post.Revised;
      postDetails.ChangeDate = post.ChangeDate;

      if (post.Keywords == "") keywords = null;
      else keywords = post.Keywords.Split(' ');

      postDetails.Keywords = keywords;

      postDetails.PostedDaysAgo = (DateTime.Now - postDetails.ChangeDate).Days;

      if (postDetails.PostedDaysAgo == 0)
      {
        TimeSpan aux = postDetails.ChangeDate.TimeOfDay;
        TimeSpan now = DateTime.Now.TimeOfDay;
        if (aux > now) postDetails.PostedDaysAgo = -1;
      }

      for (int i = 0; i < orderedComments.Count; i++)
      {
        var comment = new CommentViewModel();
        var authorComment = new UserAccount();

        try
        {
          authorComment = _userAccountRepository.GetById(orderedComments[i].UserAccountId);
          commentFiles = _commentFileRepository.GetByCommentId(orderedComments[i].Id).OrderBy(x => x.Name).ToList();
          comment.AuthorDepartment = _departmentRepository.GetDescriptionById(authorComment.DepartmentId);
          comment.AuthorPostsMade = _postRepository.GetByUserAccountId(authorComment.Id).Count();
          comment.AuthorCommentsMade = _commentRepository.GetByUserAccountId(authorComment.Id).Count();
        }
        catch (Exception) { }

        comment.Id = orderedComments[i].Id;
        comment.Description = orderedComments[i].Description;
        comment.AuthorUsername = authorComment.Name;
        comment.AuthorPhoto = authorComment.Photo;
        comment.AuthorId = orderedComments[i].UserAccountId;
        comment.ChangeDate = orderedComments[i].ChangeDate;
        comment.Revised = orderedComments[i].Revised;
        comment.Files = commentFiles;
        comments.Add(comment);
      }

      ViewBag.Comments = comments;
      ViewBag.IsModerator = CheckIsModerator();
      ViewBag.UserId = HttpContext.Session.Get<int>("_CurrentUserId");
      ViewBag.PostId = postId;
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");
      ViewBag.Post = postDetails;

      return PartialView();
    }

    [HttpGet]
    public IActionResult ShowFile(int postId, int file)
    {
      if (HttpContext.Session.Get<bool>("_IsModerator") == false)
      {
        var post = _postRepository.GetById(postId);
        if (post.Restricted == true)
        {
          var departmentId = _userAccountRepository.GetDepartmentIdById(HttpContext.Session.Get<int>("_CurrentUserId"));
          var postRestrictionCount = _postRestrictionRepository.CountByPostIdDepId(postId, departmentId);
          if (postRestrictionCount == 0)
          {
            return Redirect(Request.Host.ToString());
          }
        }
      }

      var orderedFiles = new List<PostFile>();

      try { orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList(); } catch (Exception) { }

      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + Uri.EscapeDataString(orderedFiles[file].Name));

      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPut]
    public IActionResult AprovePost(int postId)
    {
      if (!HttpContext.Request.IsAjax("PUT")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _postRepository.Aprove(postId, currentUsername); } catch (Exception) { }

      return ViewComponent("UnrevisedList");
    }

    [HttpGet]
    public IActionResult UnrevisedList()
    {
      return ViewComponent("UnrevisedList");
    }

    public bool CheckIsModerator()
    {
      if (HttpContext.Session.Get<bool>("_IsModerator") == true) return true;
      else return false;
    }
  }
}