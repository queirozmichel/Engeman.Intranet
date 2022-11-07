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
      ViewBag.FilterGrid = Request.Query["filter"];

      return View();
    }

    [HttpGet]
    public IActionResult NewQuestion()
    {
      var permissions = _userAccountRepository.GetUserPermissionsByDomainUsername(HttpContext.Session.GetString("_DomainUsername"));

      if (permissions.CreatePost == true)
      {
        ViewBag.Departments = _departmentRepository.GetAllDepartments();
        return PartialView();
      }
      else
      {
        return Ok(false);
      }
    }

    [HttpPost]
    public IActionResult NewQuestion(NewPostViewModel newPost)
    {
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newPost.Revised = true;
      }

      newPost.UserAccountId = userAccount.Id;
      newPost.DepartmentId = userAccount.DepartmentId;
      newPost.PostType = 'Q';
      newPost.CleanDescription = newPost.Description;

      _postRepository.AddQuestion(newPost);

      return View("NewQuestion");
    }

    [HttpGet]
    public IActionResult NewDocument()
    {
      var permissions = _userAccountRepository.GetUserPermissionsByDomainUsername(HttpContext.Session.GetString("_DomainUsername"));

      if (permissions.CreatePost == true)
      {
        ViewBag.DocumentOrManual = 'D';
        ViewBag.Departments = _departmentRepository.GetAllDepartments();
        return PartialView("NewDocumentManual");
      }
      else
      {
        return Ok(false);
      }
    }

    [HttpGet]
    public IActionResult NewManual()
    {
      var permissions = _userAccountRepository.GetUserPermissionsByDomainUsername(HttpContext.Session.GetString("_DomainUsername"));

      if (permissions.CreatePost == true)
      {
        ViewBag.DocumentOrManual = 'M';
        ViewBag.Departments = _departmentRepository.GetAllDepartments();
        return PartialView("NewDocumentManual");
      }
      else
      {
        return Ok(false);
      }
    }

    [HttpPost]
    public IActionResult NewDocumentManual(NewPostWithFilesViewModel newPostWithFiles, List<IFormFile> binaryData, char fileType)
    {
      if (!ModelState.IsValid || binaryData.Count == 0)
      {
        return Json(0);
      }

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newPostWithFiles.Post.Revised = true;
      }

      newPostWithFiles.Post.CleanDescription = newPostWithFiles.Post.Description;
      newPostWithFiles.Post.UserAccountId = userAccount.Id;
      newPostWithFiles.Post.DepartmentId = userAccount.DepartmentId;

      for (int i = 0; i < binaryData.Count; i++)
      {
        NewPostFileViewModel file = new NewPostFileViewModel();
        file.FileType = fileType;
        file.Name = binaryData[i].FileName;
        file.Description = newPostWithFiles.Post.Description;
        file.PostId = 0;
        if (binaryData[i].Length > 0)
        {
          using (var stream = new MemoryStream())
          {
            binaryData[i].CopyTo(stream);
            file.BinaryData = stream.ToArray();
          }
        }
        newPostWithFiles.Files.Add(file);
      }
      _postRepository.AddPostWithFile(newPostWithFiles);

      return View("NewDocumentManual");
    }

    public IActionResult BackToList(int postId)
    {
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView("ListAll");
    }

    public IQueryable<PostDto> FilterPosts(string filterGrid, string filterHeader)
    {
      var user = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      IQueryable<PostDto> posts = _postRepository.GetPostsByRestriction(user).AsQueryable();

      if (filterGrid == "allPosts")
      {
        if (user.Moderator == true)
        {
          posts = posts.Where("revised == (@0)", true);
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
    public IActionResult QuestionEdit(int idPost)
    {
      List<int> restrictedDepartments;
      var post = _postRepository.GetPostById(idPost);
      var departments = _departmentRepository.GetAllDepartments();
      ViewBag.RestrictedDepartments = null;
      ViewBag.Departments = departments;
      if (post.Restricted == true)
      {
        restrictedDepartments = _postRepository.GetRestrictedDepartmentsIdByPost(idPost);
        ViewBag.RestrictedDepartments = restrictedDepartments;
        return PartialView(post);
      }
      return PartialView(post);
    }

    [HttpGet]
    public IActionResult DocumentManualEdit(int idPost)
    {
      PostFileViewModel postFileViewModel = new PostFileViewModel();
      List<int> restrictedDepartments;
      var post = _postRepository.GetPostById(idPost);
      var orderedFiles = _postFileRepository.GetFilesByPostId(idPost).OrderBy(a => a.Name).ToList();
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      ViewBag.RestrictedDepartments = null;
      postFileViewModel.Post = post;

      for (int i = 0; i < orderedFiles.Count; i++)
      {
        postFileViewModel.Files.Add(orderedFiles[i]);
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
    public IActionResult UpdateQuestion(EditedPostViewModel editedPost)
    {
      if (!ModelState.IsValid)
      {
        return Json(0);
      }

      var currentPost = _postRepository.GetPostById(editedPost.Id);
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      editedPost.CleanDescription = editedPost.Description;

      if (currentPost.Revised == true && userAccount.NoviceUser == false)
      {
        editedPost.Revised = true;
      }

      _postRepository.UpdateQuestion(editedPost);
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView("ListAll");
    }

    [HttpPost]
    public ActionResult DocumentManualUpdate(EditedPostWithFilesViewModel editedPostWithFiles, List<IFormFile> binaryData)
    {
      var currentPost = _postRepository.GetPostById(editedPostWithFiles.Post.Id);
      char fileType = editedPostWithFiles.Files[0].FileType;
      List<NewPostFileViewModel> files = new List<NewPostFileViewModel>();
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      for (int i = 0; i < editedPostWithFiles.Files.Count; i++)
      {
        if (editedPostWithFiles.Files[i].Active == false)
        {
          _postFileRepository.DeleteFileById(editedPostWithFiles.Files[i].Id);
          editedPostWithFiles.Files.RemoveAt(i);
          i--;
        }
      }

      for (int i = 0; i < editedPostWithFiles.Files.Count; i++)
      {
        editedPostWithFiles.Files[i].FileType = fileType;
        editedPostWithFiles.Files[i].Description = editedPostWithFiles.Post.Description;
      }

      editedPostWithFiles.Post.CleanDescription = editedPostWithFiles.Post.Description;

      if (currentPost.Revised == true && userAccount.NoviceUser == false)
      {
        editedPostWithFiles.Post.Revised = true;
      }

      _postRepository.UpdatePostFile(editedPostWithFiles);

      if (binaryData.Count != 0)
      {
        files.Clear();

        for (int i = 0; i < binaryData.Count; i++)
        {
          NewPostFileViewModel newArchive = new NewPostFileViewModel();
          if (binaryData[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              binaryData[i].CopyTo(stream);
              newArchive.BinaryData = stream.ToArray();
              newArchive.Name = binaryData[i].FileName;
              newArchive.FileType = fileType;
              newArchive.Description = editedPostWithFiles.Post.Description;
              files.Add(newArchive);
            }
          }
        }
        _postRepository.AddPostFile(editedPostWithFiles.Post.Id, files);
      }
      ViewBag.FilterGrid = Request.Query["filter"];

      return PartialView("ListAll");
    }

    [HttpDelete]
    public void RemovePost(int idPost)
    {
      _postRepository.DeletePost(idPost);
    }

    [HttpGet]
    public IActionResult QuestionDetails(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var postAuthor = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(postAuthor.DepartmentId);
      var postsCount = _postRepository.GetPostsCountByUserId(postAuthor.Id);

      ViewBag.Post = post;
      ViewBag.PostAuthor = postAuthor;
      ViewBag.Department = department;
      ViewBag.PostsCount = postsCount;

      return PartialView();
    }

    [HttpGet]
    public IActionResult DocumentManualDetails(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var userAccount = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var postAuthor = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var orderedFiles = _postFileRepository.GetFilesByPostId(idPost).OrderBy(a => a.Name).ToList();
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      var postsCount = _postRepository.GetPostsCountByUserId(postAuthor.Id);

      ViewBag.Post = post;
      ViewBag.PostAuthor = postAuthor;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;
      ViewBag.Files = orderedFiles;
      ViewBag.PostsCount = postsCount;

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