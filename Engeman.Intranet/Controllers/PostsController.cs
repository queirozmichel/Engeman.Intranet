using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Engeman.Intranet.Models.ViewModels;
using System.Text;
using Engeman.Intranet.Extensions;

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

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository,
      IDepartmentRepository departmentRepository, IPostFileRepository postFileRepository, ICommentRepository postCommentRepository,
      ICommentFileRepository postCommentFileRepository, IPostRestrictionRepository postRestrictionRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _postFileRepository = postFileRepository;
      _commentRepository = postCommentRepository;
      _commentFileRepository = postCommentFileRepository;
      _postRestrictionRepository = postRestrictionRepository;
    }

    [HttpGet]
    public IActionResult Grid()
    {
      if (Request.Query["filter"] != "allPosts" && HttpContext.Session.Get<bool>("_Moderator") == false) return Redirect(Request.Host.ToString());
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView("PostsGrid");
    }

    [HttpGet]
    public IActionResult NewPost()
    {
      var permissions = new UserPermissionsViewModel();

      try { permissions = _userAccountRepository.GetUserPermissionsByUsername(HttpContext.Session.Get<string>("_Username")); }
      catch (Exception) { }
      if (permissions.CreatePost == true)
      {
        ViewBag.IsAjaxCall = isAjaxCall;
        ViewBag.Departments = _departmentRepository.Get();
        return PartialView("NewPost");
      }
      else
      {
        return Ok(StatusCodes.Status401Unauthorized);
      }
    }

    [HttpPost]
    public IActionResult NewPost(NewPostViewModel newPost, List<IFormFile> files)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_Username");
      var userAccount = new UserAccount();

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newPost.Revised = true;
      }

      newPost.CleanDescription = newPost.Description;
      newPost.UserAccountId = userAccount.Id;

      if (newPost.Keywords != null)
      {
        newPost.Keywords = newPost.Keywords.ToLower();
      }

      if (files.Count > 0)
      {
        for (int i = 0; i < files.Count; i++)
        {
          NewPostFileViewModel file = new NewPostFileViewModel();
          file.Name = files[i].FileName;
          if (files[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              files[i].CopyTo(stream);
              file.BinaryData = stream.ToArray();
            }
          }
          newPost.Files.Add(file);
        }
      }
      else
      {
        newPost.PostType = 'N';
      }

      _postRepository.AddWithLog(newPost, sessionUsername);

      return Json(1);
    }

    public IQueryable<PostGridViewModel> FilterPosts(string filterGrid, string filterHeader)
    {
      var user = _userAccountRepository.GetById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      IQueryable<PostGridViewModel> posts = _postRepository.GetByRestriction(user).AsQueryable();
      List<Comment> comments = new List<Comment>();

      try
      {
        user = _userAccountRepository.GetById(HttpContext.Session.Get<int>("_CurrentUserId"));
        posts = _postRepository.GetByRestriction(user).AsQueryable();
      }
      catch (Exception) { }
      foreach (var post in posts)
      {
        comments = _commentRepository.GetByPostId(post.Id);
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
        if (user.Moderator == true)
        {
          posts = posts.Where("revised == (@0) && UnrevisedComments == (@1) ", true, false);
        }
      }
      else if (filterGrid == "unrevisedPosts")
      {
        posts = posts.Where("revised == (@0)", false);
      }
      else if (filterGrid == "unrevisedComments")
      {
        posts = _postRepository.GetWithUnrevisedComments().AsQueryable();
      }

      if (filterHeader == "manual")
      {
        return posts = posts.Where("postType == (@0)", "M");
      }
      else if (filterHeader == "document")
      {
        return posts = posts.Where("postType == (@0)", "D");
      }
      else if (filterHeader == "no-attachment")
      {
        return posts = posts.Where(x => x.PostType == 'N');
      }
      else if (filterHeader == "my")
      {
        return posts = posts.Where(x => x.UserAccountId == user.Id);
      }
      else
      {
        return posts;
      }
    }

    public IQueryable<PostGridViewModel> FilterPostsBySearchPhrase(IQueryable<PostGridViewModel> posts, string searchPhrase)
    {
      posts = posts.Where("userAccountName.Contains(@0, StringComparison.OrdinalIgnoreCase) OR department.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "subject.Contains(@0, StringComparison.OrdinalIgnoreCase) OR keywords.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "changeDate.Contains(@0)", searchPhrase);
      return posts;
    }

    public IQueryable OrderedPosts(IQueryable<PostGridViewModel> posts, string orderedField, int current, int rowCount)
    {
      IQueryable aux;
      if (orderedField.Contains("changeDate asc"))
      {
        return aux = posts.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      }
      else if (orderedField.Contains("changeDate desc"))
      {
        return aux = posts.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      }
      else
      {
        return aux = posts.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
      }
    }

    [HttpPost]
    public JsonResult DataGrid(string filterGrid, string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<PostGridViewModel> posts = null;
      IQueryable paginatedPosts;
      int total = 0;
      bool isModerator = checkIsModerator();
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      string orderedField = String.Format("{0} {1}", field, order);

      posts = FilterPosts(filterGrid, filterHeader);
      total = posts.Count();

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        posts = FilterPostsBySearchPhrase(posts, searchPhrase);
        total = posts.Count();
      }

      if (rowCount == -1)
      {
        rowCount = total;
      }

      paginatedPosts = OrderedPosts(posts, orderedField, current, rowCount);

      return Json(new { rows = paginatedPosts, current, rowCount, total, isModerator });
    }

    [HttpGet]
    public IActionResult EditPost(int postId)
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      List<int> restrictedDepartments;
      PostEditViewModel postEditViewModel = new PostEditViewModel();
      var orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList();
      var post = _postRepository.GetById(postId);
      var departments = _departmentRepository.Get();
      ViewBag.IsAjaxCall = isAjaxCall;
      ViewBag.RestrictedDepartments = null;
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
        restrictedDepartments = _postRestrictionRepository.GetDepartmentsByIdPost(postId);
        ViewBag.RestrictedDepartments = restrictedDepartments;
        return PartialView(postEditViewModel);
      }
      return PartialView(postEditViewModel);
    }

    [HttpPut]
    public IActionResult UpdatePost(PostEditViewModel editedPost, List<IFormFile> binaryData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_Username");
      var fileList = new List<NewPostFileViewModel>();
      var currentPost = new Post();
      var userAccount = new UserAccount();

      editedPost.CleanDescription = editedPost.Description;
      if (editedPost.Keywords != null)
      {
        editedPost.Keywords = editedPost.Keywords.ToLower();
      }

      if (editedPost.Files.Count > 0)
      {
        for (int i = 0; i < editedPost.Files.Count; i++)
        {
          if (editedPost.Files[i].Active == false)
          {
            _postFileRepository.Delete(editedPost.Files[i].Id);
            editedPost.Files.RemoveAt(i);
            i--;
          }
        }
      }
      else
      {
        editedPost.PostType = 'N';
      }

      if (binaryData.Count != 0)
      {
        fileList.Clear();

        for (int i = 0; i < binaryData.Count; i++)
        {
          NewPostFileViewModel newFile = new NewPostFileViewModel();
          if (binaryData[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              binaryData[i].CopyTo(stream);
              newFile.BinaryData = stream.ToArray();
              newFile.Name = binaryData[i].FileName;
              fileList.Add(newFile);
            }
          }
        }
        _postFileRepository.Add(editedPost.Id, fileList);
      }

      if (currentPost.Revised == true && userAccount.NoviceUser == false)
      {
        editedPost.Revised = true;
      }

      _postRepository.UpdateWithLog(editedPost, sessionUsername);
      ViewBag.FilterGrid = Request.Query["filter"];

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public IActionResult RemovePost(int postId)
    {
      var currentUsername = HttpContext.Session.Get<string>("_Username");

      return PartialView("PostsGrid");
    }

    [HttpGet]
    public IActionResult PostDetails(int postId)
    {
      PostDetailsViewModel postDetails = new PostDetailsViewModel();
      List<CommentFile> commentFiles = new List<CommentFile>();
      var comments = new List<CommentViewModel>();
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      var post = _postRepository.GetById(postId);
      var postAuthor = _userAccountRepository.GetById(post.UserAccountId);
      var orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList();
      var department = _departmentRepository.GetById(postAuthor.DepartmentId);
      var postsCount = _postRepository.CountByUserId(postAuthor.Id);
      var commentsCount = _commentRepository.CountByUserId(postAuthor.Id);
      var userAccount = _userAccountRepository.GetById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var orderedComments = _commentRepository.GetByRestriction(userAccount, postId);
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
        orderedComments = _commentRepository.GetByRestriction(userAccount, postId);
      }
      catch (Exception) { }
      postDetails.Id = post.Id;
      postDetails.Subject = post.Subject;
      postDetails.Description = post.Description;
      postDetails.Files = orderedFiles;

      if (post.Keywords == "")
      {
        keywords = null;
      }
      else
      {
        keywords = post.Keywords.Split(';');
      }
      postDetails.Keywords = keywords;
      postDetails.Revised = post.Revised;
      postDetails.ChangeDate = post.ChangeDate;
      postDetails.PostedDaysAgo = (DateTime.Now - postDetails.ChangeDate).Days;
      if (postDetails.PostedDaysAgo == 0)
      {
        TimeSpan aux = postDetails.ChangeDate.TimeOfDay;
        TimeSpan now = DateTime.Now.TimeOfDay;
        if (aux > now)
        {
          postDetails.PostedDaysAgo = -1;
        }
      }
      postDetails.AuthorId = postAuthor.Id;
      postDetails.AuthorUsername = postAuthor.Name;
      postDetails.AuthorDepartment = department.Description;
      postDetails.AuthorPostsMade = postsCount;
      postDetails.AuthorCommentsMade = commentsCount;
      postDetails.AuthorPhoto = postAuthor.Photo;

      for (int i = 0; i < orderedComments.Count; i++)
      {
        var comment = new CommentViewModel();
        var authorComment = _userAccountRepository.GetById(orderedComments[i].UserAccountId);
        commentFiles = _commentFileRepository.GetByCommentId(orderedComments[i].Id).OrderBy(x => x.Name).ToList();
        comment.Id = orderedComments[i].Id;
        comment.Description = orderedComments[i].Description;
        comment.AuthorUsername = authorComment.Name;
        comment.AuthorPhoto = authorComment.Photo;
        comment.AuthorId = orderedComments[i].UserAccountId;
        comment.AuthorDepartment = _departmentRepository.GetDescriptionById(authorComment.DepartmentId);
        comment.AuthorPostsMade = _postRepository.GetByUserAccountId(authorComment.Id).Count();
        comment.AuthorCommentsMade = _commentRepository.GetByUserAccountId(authorComment.Id).Count();
        comment.ChangeDate = orderedComments[i].ChangeDate;
        comment.Revised = orderedComments[i].Revised;
        comment.Files = commentFiles;
        comments.Add(comment);
      }

      ViewBag.Comments = comments;
      ViewBag.IsModerator = checkIsModerator();
      ViewBag.UserId = HttpContext.Session.Get<int>("_CurrentUserId");
      ViewBag.PostId = postId;
      ViewBag.IsAjaxCall = isAjaxCall;
      ViewBag.Post = postDetails;

      return PartialView();
    }

    [HttpGet]
    public IActionResult ShowFile(int postId, int file)
    {
      var orderedFiles = _postFileRepository.GetByPostId(postId).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + Uri.EscapeDataString(orderedFiles[file].Name));
      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPut]
    public IActionResult AprovePost(int postId)
    {
      var currentUsername = HttpContext.Session.Get<string>("_Username");

      return ViewComponent("UnrevisedList");
    }

    [HttpGet]
    public IActionResult UnrevisedList()
    {
      return ViewComponent("UnrevisedList");
    }

    public bool checkIsModerator()
    {
      if (HttpContext.Session.Get<bool>("_Moderator") == true) return true;
      else return false;
    }
  }
}