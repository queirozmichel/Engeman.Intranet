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
  public class PostsController : RootController
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPostFileRepository _postFileRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentFileRepository _commentFileRepository;
    private readonly IPostRestrictionRepository _postRestrictionRepository;
    private readonly IKeywordRepository _keywordRepository;
    private readonly IPostKeywordRepository _postKeywordRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository,
      IDepartmentRepository departmentRepository, IPostFileRepository postFileRepository, ICommentRepository postCommentRepository,
      ICommentFileRepository postCommentFileRepository, IPostRestrictionRepository postRestrictionRepository, IConfiguration configuration,
      IKeywordRepository keywordRepository, IPostKeywordRepository postKeywordRepository, INotificationRepository notificationRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _postFileRepository = postFileRepository;
      _commentRepository = postCommentRepository;
      _commentFileRepository = postCommentFileRepository;
      _postRestrictionRepository = postRestrictionRepository;
      _configuration = configuration;
      _keywordRepository = keywordRepository;
      _postKeywordRepository = postKeywordRepository;
      _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public IActionResult Grid()
    {
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");

      return PartialView("PostsGrid");
    }

    [HttpPost]
    public JsonResult DataGrid(string filterGrid, string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<PostGridViewModel> posts = null;
      IQueryable paginatedPosts;
      var total = 0;
      var isModerator = HttpContext.Session.Get<bool>("_IsModerator");
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

      if (GlobalFunctions.IsModerator(user.Id))
      {
        posts = posts.OrderBy("revised == (@0)", true).ThenBy("unrevisedComments == (@0)", false);
      }
      else
      {
        posts = posts.Where("revised == (@0) || UnrevisedComments == (@1) || userAccountId == (@2)", true, false, HttpContext.Session.Get<int>("_CurrentUserId"));
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

      try
      {
        permissions = _userAccountRepository.GetPermissionsById(HttpContext.Session.Get<int>("_CurrentUserId")).DeserializeAndConvertIntToBool<UserPermissionsViewModel>();
      }
      catch (Exception) { }

      if (permissions.PostType.Informative.CanPost == true || permissions.PostType.Question.CanPost == true
        || permissions.PostType.Document.CanPost == true || permissions.PostType.Manual.CanPost == true)
      {
        ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");
        try { ViewBag.Departments = _departmentRepository.Get(); }
        catch (Exception) { }
        return PartialView("NewPost", permissions);
      }
      else return StatusCode(StatusCodes.Status401Unauthorized);
    }

    [HttpPost]
    public IActionResult NewPost(NewPostViewModel newPost, List<IFormFile> addFiles)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      newPost.UserAccountId = HttpContext.Session.Get<int>("_CurrentUserId");
      newPost.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(newPost.Description));
      newPost.Revised = !GlobalFunctions.RequiresModeration(newPost.UserAccountId, newPost.PostType);

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

      if (newPost.Keywords is not null) newPost.KeywordsList = _keywordRepository.GetByIds(newPost.Keywords.Split(";").Select(int.Parse).ToArray());

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
      if (!IsAuthorized(postId, 2)) return Json(StatusCodes.Status401Unauthorized);

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

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");
      ViewBag.Departments = departments;
      postEditViewModel.Id = post.Id;
      postEditViewModel.Restricted = post.Restricted;
      postEditViewModel.Subject = post.Subject;
      postEditViewModel.Description = post.Description;
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
      var currentUserId = HttpContext.Session.Get<int>("_CurrentUserId");
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

      if (editedPost.Keywords is not null) editedPost.KeywordsList = _keywordRepository.GetByIds(editedPost.Keywords.Split(";").Select(int.Parse).ToArray());

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

      if (!GlobalFunctions.RequiresModeration(currentUserId, editedPost.PostType)) editedPost.Revised = true;

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
      if (!HttpContext.Request.IsAjaxOrFetch("DELETE")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _postRepository.Delete(postId, currentUsername); } catch (Exception) { }

      return Json(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult PostDetails(int postId)
    {
      if (!IsAuthorized(postId, 1))
      {
        return Redirect(Request.Host.ToString());
      }

      UserPermissionsViewModel permissions = new();
      var currentUserId = HttpContext.Session.Get<int>("_CurrentUserId");
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
        permissions = _userAccountRepository.GetPermissionsById(currentUserId).DeserializeAndConvertIntToBool<UserPermissionsViewModel>();
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
      postDetails.PostType = post.PostType;
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
      keywords = _postKeywordRepository.GetKeywordsByPostId(postId);
      postDetails.Keywords = keywords;
      postDetails.PostedDaysAgo = GlobalFunctions.DaysUntilToday(postDetails.ChangeDate);

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
      ViewBag.IsModerator = GlobalFunctions.IsModerator(HttpContext.Session.Get<int>("_CurrentUserId"));
      ViewBag.UserId = HttpContext.Session.Get<int>("_CurrentUserId");
      ViewBag.PostId = postId;
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");
      ViewBag.Post = postDetails;
      ViewBag.Permissions = permissions;
      ViewBag.CanComment = IsAuthorized(postId, 4);
      ViewBag.CanEdit = IsAuthorized(postId, 2);
      ViewBag.CanDelete = IsAuthorized(postId, 3);

      return PartialView();
    }

    [HttpGet]
    public IActionResult ShowFile(int postId, int file)
    {
      if (!HttpContext.Session.Get<bool>("_IsModerator"))
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
      if (!HttpContext.Request.IsAjaxOrFetch("PUT")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _postRepository.Aprove(postId, currentUsername); } catch (Exception) { }

      return Json(StatusCodes.Status200OK);
    }

    public string PostDeleteAuthorization(int postId)
    {
      var currentUserId = HttpContext.Session.Get<int>("_CurrentUserId");
      Post post = new();
      UserPermissionsViewModel permissions = new();

      try
      {
        permissions = _userAccountRepository.GetPermissionsById(currentUserId).DeserializeAndConvertIntToBool<UserPermissionsViewModel>();
        post = _postRepository.GetById(postId);
      }
      catch (Exception) { throw; }

      if (!GlobalFunctions.IsModerator(currentUserId))
      {
        if (currentUserId != post.UserAccountId)
        {
          if (permissions.PostType.Informative.DeleteAnyPost == false && permissions.PostType.Question.DeleteAnyPost == false
            && permissions.PostType.Document.DeleteAnyPost == false && permissions.PostType.Manual.DeleteAnyPost == false) return "NotAnyPost";
          else
          {
            if (post.PostType == 'I')
            {
              if (permissions.PostType.Informative.DeleteAnyPost == false) return "NotInformativePost";
            }
            else if (post.PostType == 'Q')
            {
              if (permissions.PostType.Question.DeleteAnyPost == false) return "NotQuestionPost";
            }
            else if (post.PostType == 'D')
            {
              if (permissions.PostType.Document.DeleteAnyPost == false) return "NotDocumentPost";
            }
            else if (post.PostType == 'M')
            {
              if (permissions.PostType.Manual.DeleteAnyPost == false) return "NotManualPost";
            }
          }
        }
      }
      return "DeletePost";
    }

    /// <summary>
    /// Verifica se o usuário corrente está autorizado a executar determinada ação de acordo com o nível de permissão.
    /// </summary>
    /// <param name="postId">ID da postagem na qual é solicitado acesso</param>
    /// <param name="action">1 = Detalhar a postagem | 2 = Editar a postagem | 3 = Editar a postagem | 4 = Comentar a postagem</param>
    /// <returns></returns>
    public bool IsAuthorized(int postId, int action)
    {
      var currentUserId = HttpContext.Session.Get<int>("_CurrentUserId");
      var departmentId = _userAccountRepository.GetDepartmentIdById(currentUserId);
      var restrictedDept = _postRestrictionRepository.CountByPostIdDepId(postId, departmentId);
      var postAux = _postRepository.GetById(postId);
      UserPermissionsViewModel permissions = new();

      try { permissions = _userAccountRepository.GetPermissionsById(currentUserId).DeserializeAndConvertIntToBool<UserPermissionsViewModel>(); }
      catch (Exception) { throw; }

      //Verifica se tem permissão de visualizar a postagem
      if (action == 1)
      {
        if (!GlobalFunctions.IsModerator(currentUserId))
        {
          if (postAux.UserAccountId != currentUserId)
          {
            if (postAux.Restricted == true && restrictedDept == 0)
            {
              return false;
            }
            if (postAux.Revised == false)
            {
              return false;
            }
          }
        }
        return true;
      }
      //Verifica se tem permissão de editar a postagem
      else if (action == 2)
      {
        if (!GlobalFunctions.IsModerator(currentUserId))
        {
          if (currentUserId != postAux.UserAccountId)
          {
            if (postAux.Restricted == true && restrictedDept == 0)
            {
              return false;
            }

            if (postAux.Revised == false)
            {
              return false;
            }

            if (permissions.PostType.Informative.EditAnyPost == false && permissions.PostType.Question.EditAnyPost == false && permissions.PostType.Document.EditAnyPost == false && permissions.PostType.Manual.EditAnyPost == false)
            {
              return false;
            }
            else
            {
              if (postAux.PostType == 'I')
              {
                if (permissions.PostType.Informative.EditAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'Q')
              {
                if (permissions.PostType.Question.EditAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'D')
              {
                if (permissions.PostType.Document.EditAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'M')
              {
                if (permissions.PostType.Manual.EditAnyPost == false)
                {
                  return false;
                }
              }
            }
          }
        }
      }
      else if (action == 3)
      {
        if (!GlobalFunctions.IsModerator(currentUserId))
        {
          if (currentUserId != postAux.UserAccountId)
          {
            if (postAux.Restricted == true && restrictedDept == 0)
            {
              return false;
            }

            if (postAux.Revised == false)
            {
              return false;
            }

            if (permissions.PostType.Informative.DeleteAnyPost == false && permissions.PostType.Question.DeleteAnyPost == false && permissions.PostType.Document.DeleteAnyPost == false && permissions.PostType.Manual.DeleteAnyPost == false)
            {
              return false;
            }
            else
            {
              if (postAux.PostType == 'I')
              {
                if (permissions.PostType.Informative.DeleteAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'Q')
              {
                if (permissions.PostType.Question.DeleteAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'D')
              {
                if (permissions.PostType.Document.DeleteAnyPost == false)
                {
                  return false;
                }
              }
              else if (postAux.PostType == 'M')
              {
                if (permissions.PostType.Manual.DeleteAnyPost == false)
                {
                  return false;
                }
              }
            }
          }
        }
      }
      //Verifica se tem permissão de comentar a postagem
      else if (action == 4)
      {
        if (!GlobalFunctions.IsModerator(currentUserId))
        {
          if (postAux.UserAccountId != currentUserId)
          {
            if (postAux.Restricted == true && restrictedDept == 0)
            {
              return false;
            }
            if (postAux.PostType == 'I')
            {
              if (permissions.PostType.Informative.CanComment == false)
              {
                return false;
              }
            }
            else if (postAux.PostType == 'Q')
            {
              if (permissions.PostType.Question.CanComment == false)
              {
                return false;
              }
            }
            else if (postAux.PostType == 'D')
            {
              if (permissions.PostType.Document.CanComment == false)
              {
                return false;
              }
            }
            else if (postAux.PostType == 'M')
            {
              if (permissions.PostType.Manual.CanComment == false)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
  }
}