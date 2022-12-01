using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class PostsController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPostFileRepository _postFileRepository;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository,
      IDepartmentRepository departmentRepository, IPostFileRepository postFileRepository, IPostCommentRepository postCommentRepository,
      IPostCommentFileRepository postCommentFileRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _postFileRepository = postFileRepository;
      _postCommentRepository = postCommentRepository;
      _postCommentFileRepository = postCommentFileRepository;
    }

    [HttpGet]
    public IActionResult ListAll()
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

      ViewBag.IsAjaxCall = isAjaxCall;
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView();
    }

    [HttpGet]
    public IActionResult NewPost()
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      var permissions = _userAccountRepository.GetUserPermissionsByDomainUsername(HttpContext.Session.GetString("_DomainUsername"));
      if (permissions.CreatePost == true)
      {
        ViewBag.IsAjaxCall = isAjaxCall;
        ViewBag.Departments = _departmentRepository.GetAllDepartments();
        return PartialView("NewPost");
      }
      else
      {
        return Ok(0);
      }
    }

    [HttpPost]
    public IActionResult NewPost(NewPostViewModel newPost, List<IFormFile> files)
    {
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newPost.Revised = true;
      }

      newPost.CleanDescription = newPost.Description;
      newPost.UserAccountId = userAccount.Id;
      newPost.DepartmentId = userAccount.DepartmentId;
      newPost.PostType = 'Q';

      if (files.Count > 0)
      {
        for (int i = 0; i < files.Count; i++)
        {
          NewPostFileViewModel file = new NewPostFileViewModel();
          file.FileType = newPost.FileType;
          newPost.PostType = 'F';
          file.Name = files[i].FileName;
          file.Description = newPost.Description;
          file.PostId = 0;
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
      _postRepository.AddPost(newPost);
      return Json(1);
    }

    public IQueryable<PostDto> FilterPosts(string filterGrid, string filterHeader)
    {
      var user = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      IQueryable<PostDto> posts = _postRepository.GetPostsByRestriction(user).AsQueryable();
      List<PostComment> comments = new List<PostComment>();

      foreach (var post in posts)
      {
        comments = _postCommentRepository.GetPostCommentsByPostId(post.Id);
        for (int i = 0; i < comments.Count; i++)
        {
          if (comments[i].Revised == false)
          {
            post.HasUnrevisedComments = true;
            break;
          }
        }
      }

      if (filterGrid == "allPosts")
      {
        if (user.Moderator == true)
        {
          posts = posts.Where("revised == (@0) && HasUnrevisedComments == (@1) ", true, false);
        }
      }
      else if (filterGrid == "unrevisedPosts")
      {
        posts = posts.Where("revised == (@0)", false);
      }
      else if (filterGrid == "unrevisedComments")
      {
        posts = _postRepository.GetPostsWithUnrevisedComments().AsQueryable();
      }

      if (filterHeader == "manual")
      {
        return posts = posts.Where("fileType == (@0)", "M");
      }
      else if (filterHeader == "document")
      {
        return posts = posts.Where("fileType == (@0)", "D");
      }
      else if (filterHeader == "question")
      {
        return posts = posts.Where(x => x.PostType == 'Q');
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

    public IQueryable<PostDto> FilterPostsBySearchPhrase(IQueryable<PostDto> posts, string searchPhrase)
    {
      int id = 0;
      int.TryParse(searchPhrase, out id);
      posts = posts.Where("userAccountName.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "departmentDescription.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "subject.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "cleanDescription.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
        "changeDate.Contains(@0) OR id == (@1)", searchPhrase, id);
      return posts;
    }

    public IQueryable OrderedPosts(IQueryable<PostDto> posts, string orderedField, int current, int rowCount)
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
    public JsonResult GetDataGrid(string filterGrid, string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<PostDto> posts = null;
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
    public IActionResult EditPost(int idPost)
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      List<int> restrictedDepartments;
      PostFileViewModel postFileViewModel = new PostFileViewModel();
      var orderedFiles = _postFileRepository.GetFilesByPostId(idPost).OrderBy(a => a.Name).ToList();
      var post = _postRepository.GetPostById(idPost);
      var departments = _departmentRepository.GetAllDepartments();
      ViewBag.IsAjaxCall = isAjaxCall;
      ViewBag.RestrictedDepartments = null;
      ViewBag.Departments = departments;
      postFileViewModel.Post = post;

      if (orderedFiles.Count != 0)
      {
        for (int i = 0; i < orderedFiles.Count; i++)
        {
          postFileViewModel.Files.Add(orderedFiles[i]);
        }
      }

      if (post.Restricted == true)
      {
        restrictedDepartments = _postRepository.GetRestrictedDepartmentsIdByPost(idPost);
        ViewBag.RestrictedDepartments = restrictedDepartments;
        return PartialView(postFileViewModel);
      }
      return PartialView(postFileViewModel);
    }

    [HttpPost]
    public IActionResult UpdatePost(EditedPostViewModel editedPost, List<IFormFile> files)
    {
      List<NewPostFileViewModel> fileList = new List<NewPostFileViewModel>();
      var currentPost = _postRepository.GetPostById(editedPost.Id);
      char fileType = ' ';
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      if (editedPost.Files.Count > 0)
      {
        fileType = editedPost.Files[0].FileType;
        for (int i = 0; i < editedPost.Files.Count; i++)
        {
          if (editedPost.Files[i].Active == false)
          {
            _postFileRepository.DeleteFileById(editedPost.Files[i].Id);
            editedPost.Files.RemoveAt(i);
            i--;
          }
        }
        for (int i = 0; i < editedPost.Files.Count; i++)
        {
          editedPost.Files[i].FileType = fileType;
          editedPost.Files[i].Description = editedPost.Description;
        }
      }

      editedPost.CleanDescription = editedPost.Description;

      if (files.Count != 0)
      {
        fileList.Clear();

        for (int i = 0; i < files.Count; i++)
        {
          NewPostFileViewModel newFile = new NewPostFileViewModel();
          if (files[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              files[i].CopyTo(stream);
              newFile.BinaryData = stream.ToArray();
              newFile.Name = files[i].FileName;
              newFile.FileType = fileType;
              newFile.Description = editedPost.Description;
              fileList.Add(newFile);
            }
          }
        }
        _postRepository.AddPostFile(editedPost.Id, fileList);
      }

      if (currentPost.Revised == true && userAccount.NoviceUser == false)
      {
        editedPost.Revised = true;
      }

      _postRepository.UpdatePost(editedPost);
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView("ListAll");
    }

    [HttpDelete]
    public IActionResult RemovePost(int idPost)
    {
      _postRepository.DeletePost(idPost);
      return PartialView("ListAll");
    }

    [HttpGet]
    public IActionResult PostDetails(int idPost)
    {
      PostDetailsViewModel postDetails = new PostDetailsViewModel();
      List<CommentFile> commentFiles = new List<CommentFile>();
      var comments = new List<CommentViewModel>();
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      var post = _postRepository.GetPostById(idPost);
      var postAuthor = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var orderedFiles = _postFileRepository.GetFilesByPostId(idPost).OrderBy(a => a.Name).ToList();
      var department = _departmentRepository.GetDepartmentById(postAuthor.DepartmentId);
      var postsCount = _postRepository.GetPostsByUserId(postAuthor.Id).Count();
      var commentsCount = _postCommentRepository.GetPostCommentsByUserId(postAuthor.Id).Count();
      var userAccount = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var orderedComments = _postCommentRepository.GetPostCommentsByRestriction(userAccount, idPost);
      string[] keywords;

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
        var authorComment = _userAccountRepository.GetUserAccountById(orderedComments[i].UserAccountId);
        commentFiles = _postCommentFileRepository.GetFilesByPostCommentId(orderedComments[i].Id).OrderBy(x => x.Name).ToList();
        comment.Id = orderedComments[i].Id;
        comment.Description = orderedComments[i].Description;
        comment.AuthorUsername = authorComment.Name;
        comment.AuthorPhoto = authorComment.Photo;
        comment.AuthorId = orderedComments[i].UserAccountId;
        comment.AuthorDepartment = _departmentRepository.GetDepartmentNameById(orderedComments[i].DepartmentId);
        comment.AuthorPostsMade = _postRepository.GetPostsByUserId(authorComment.Id).Count();
        comment.AuthorCommentsMade = _postCommentRepository.GetPostCommentsByUserId(authorComment.Id).Count();
        comment.ChangeDate = orderedComments[i].ChangeDate;
        comment.Revised = orderedComments[i].Revised;
        comment.Files = commentFiles;
        comments.Add(comment);
      }

      ViewBag.Comments = comments;
      ViewBag.IsModerator = checkIsModerator();
      ViewBag.UserId = HttpContext.Session.GetInt32("_UserAccountId");
      ViewBag.PostId = idPost;
      ViewBag.IsAjaxCall = isAjaxCall;
      ViewBag.Post = postDetails;

      return PartialView();
    }    

    [HttpGet]
    public ActionResult ShowFile(int idPost, int file)
    {
      var orderedFiles = _postFileRepository.GetFilesByPostId(idPost).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + orderedFiles[file].Name);

      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult NewComment(NewCommentViewModel newComment, List<IFormFile> files)
    {
      if (!ModelState.IsValid)
      {
        return Json(-1);
      }

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newComment.Revised = true;
      }

      newComment.UserAccountId = (int)HttpContext.Session.GetInt32("_UserAccountId");
      newComment.DepartmentId = (int)HttpContext.Session.GetInt32("_DepartmentId");
      newComment.CleanDescription = newComment.Description;

      if (files.Count > 0)
      {
        for (int i = 0; i < files.Count; i++)
        {
          NewCommentFileViewModel file = new NewCommentFileViewModel();
          file.Name = files[i].FileName;
          file.Description = newComment.Description;
          if (files[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              files[i].CopyTo(stream);
              file.BinaryData = stream.ToArray();
            }
          }
          newComment.Files.Add(file);
        }
      }
      _postCommentRepository.AddPostComment(newComment);

      return Json("ListAll");
    }

    [HttpDelete]
    public IActionResult DeleteComment(int idComment)
    {
      var result = _postCommentRepository.DeletePostCommentById(idComment);
      return Json(result);
    }

    [HttpPut]
    public IActionResult AproveComment(int idComment)
    {
      var comment = _postCommentRepository.GetPostCommentById(idComment);
      comment.Revised = true;
      _postCommentRepository.UpdatePostCommentById(idComment, comment);

      return ViewComponent("UnrevisedList");
    }

    [HttpPut]
    public IActionResult AprovePost(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      post.Revised = true;
      _postRepository.UpdatePost(idPost, post);

      return ViewComponent("UnrevisedList");
    }

    [HttpGet]
    public IActionResult UnrevisedList()
    {
      return ViewComponent("UnrevisedList");
    }

    public bool checkIsModerator()
    {
      if (HttpContext.Session.GetInt32("_Moderator") == 1)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }
}