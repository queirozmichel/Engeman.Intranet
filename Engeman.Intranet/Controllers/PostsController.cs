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

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class PostsController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IArchiveRepository _archiveRepository;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository,
      IDepartmentRepository departmentRepository, IArchiveRepository archiveRepository, IPostCommentRepository postCommentRepository,
      IPostCommentFileRepository postCommentFileRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _archiveRepository = archiveRepository;
      _postCommentRepository = postCommentRepository;
      _postCommentFileRepository = postCommentFileRepository;
    }

    [HttpGet]
    public IActionResult ListAll()
    {
      return View();
    }

    [HttpGet]
    public IActionResult NewQuestion()
    {
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      return View();
    }

    [HttpPost]
    public IActionResult NewQuestion(AskQuestionDto askQuestionDto)
    {
      if (!ModelState.IsValid)
      {
        return Json(0);
      }

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      askQuestionDto.UserAccountId = userAccount.Id;
      askQuestionDto.DepartmentId = userAccount.DepartmentId;
      askQuestionDto.PostType = 'Q';
      askQuestionDto.Active = 'S';
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.DomainAccount = sessionDomainUsername;

      _postRepository.AddQuestion(askQuestionDto);

      return View("NewQuestion");
    }

    [HttpGet]
    public IActionResult NewDocument()
    {
      ViewBag.DocumentOrManual = 'D';
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      return View("NewDocumentManual");
    }

    [HttpGet]
    public IActionResult NewManual()
    {
      ViewBag.DocumentOrManual = 'M';
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      return View("NewDocumentManual");
    }

    [HttpPost]
    public IActionResult NewDocumentManual(PostArchiveDto postArchiveDto, List<IFormFile> binaryData, char fileType)
    {
      if (!ModelState.IsValid || binaryData.Count == 0)
      {
        return Json(0);
      }

      AskQuestionDto askQuestionDto = new AskQuestionDto();
      List<Archive> archiveList = new List<Archive>();

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      askQuestionDto.Restricted = postArchiveDto.Post.Restricted;
      askQuestionDto.Subject = postArchiveDto.Post.Subject;
      askQuestionDto.Description = postArchiveDto.Post.Description;
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.Keywords = postArchiveDto.Post.Keywords;
      askQuestionDto.DepartmentsList = postArchiveDto.DepartmentsList;
      askQuestionDto.UserAccountId = userAccount.Id;
      askQuestionDto.DomainAccount = sessionDomainUsername;
      askQuestionDto.DepartmentId = userAccount.DepartmentId;
      askQuestionDto.PostType = 'A';

      for (int i = 0; i < binaryData.Count; i++)
      {
        Archive archive = new Archive();
        archive.ArchiveType = fileType;
        archive.Name = binaryData[i].FileName;
        archive.Description = postArchiveDto.Post.Description;
        archive.PostId = 0;
        if (binaryData[i].Length > 0)
        {
          using (var stream = new MemoryStream())
          {
            binaryData[i].CopyTo(stream);
            archive.BinaryData = stream.ToArray();
          }
        }
        archiveList.Add(archive);
      }

      _postRepository.AddArchive(askQuestionDto, archiveList);

      return View("NewDocumentManual");
    }

    public IActionResult BackToList()
    {
      return PartialView("ListAll");
    }

    public IQueryable<PostDto> FilterPosts(string filter)
    {
      int departmentIdSession = (int)HttpContext.Session.GetInt32("_DepartmentId");
      int userIdSession = (int)HttpContext.Session.GetInt32("_UserAccountId");
      IQueryable<PostDto> posts = _postRepository.GetPostsByRestriction(departmentIdSession, userIdSession).AsQueryable();

      if (filter == "manual")
      {
        return posts = posts.Where("archiveType == (@0)", "M");
      }
      if (filter == "document")
      {
        return posts = posts.Where("archiveType == (@0)", "D");
      }
      if (filter == "question")
      {
        return posts = posts.Where(x => x.PostType == 'Q');
      }
      if (filter == "my")
      {
        return posts = posts.Where(x => x.UserAccountId == userIdSession);
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
    public JsonResult GetDataGrid(string filter, int rowCount, string searchPhrase, int current)
    {
      IQueryable<PostDto> posts = null;
      IQueryable paginatedPosts;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      string orderedField = String.Format("{0} {1}", field, order);

      posts = FilterPosts(filter);
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

      return Json(new { rows = paginatedPosts, current, rowCount, total });
    }   

    [HttpGet]
    public IActionResult QuestionEdit(int idPost)
    {
      List<int> restrictedDepartments;
      var post = _postRepository.GetPostById(idPost);
      var departments = _departmentRepository.GetAllDepartments();
      ViewBag.RestrictedDepartments = null;
      ViewBag.Departments = departments;
      if (post.Restricted == 'S')
      {
        restrictedDepartments = _postRepository.GetRestrictedDepartmentsIdByPost(idPost);
        ViewBag.RestrictedDepartments = restrictedDepartments;
        return PartialView(post);
      }
      return PartialView(post);
    }

    [HttpGet]
    public IActionResult ArchivePostEdit(int idPost)
    {
      PostArchiveViewModel postArchiveViewModel = new PostArchiveViewModel();
      List<int> restrictedDepartments;
      var post = _postRepository.GetPostById(idPost);
      var orderedArchives = _archiveRepository.GetArchiveByPostId(idPost).OrderBy(a => a.Name).ToList();
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      ViewBag.RestrictedDepartments = null;
      postArchiveViewModel.Post = post;

      for (int i = 0; i < orderedArchives.Count; i++)
      {
        postArchiveViewModel.Archive.Add(orderedArchives[i]);
      }

      if (post.Restricted == 'S')
      {
        restrictedDepartments = _postRepository.GetRestrictedDepartmentsIdByPost(idPost);
        ViewBag.RestrictedDepartments = restrictedDepartments;
        return PartialView(postArchiveViewModel);
      }
      return PartialView(postArchiveViewModel);
    }

    [HttpPost]
    public IActionResult UpdateQuestion(AskQuestionDto askQuestionDto)
    {
      if (!ModelState.IsValid)
      {
        return Json(0);
      }

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      askQuestionDto.UserAccountId = userAccount.Id;
      askQuestionDto.DepartmentId = userAccount.DepartmentId;
      askQuestionDto.PostType = 'Q';
      askQuestionDto.Active = 'S';
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.DomainAccount = sessionDomainUsername;

      _postRepository.UpdateQuestion(askQuestionDto.Id, askQuestionDto);
      return PartialView("ListAll");
    }

    [HttpPost]
    public ActionResult UpdateArchive(PostArchiveViewModel postArchives, List<IFormFile> binaryData)
    {
      if (!ModelState.IsValid)
      {
        return Json(0);
      }

      char archiveType = postArchives.Archive[0].ArchiveType;
      AskQuestionDto postInformation = new AskQuestionDto();
      List<Archive> archives = new List<Archive>();

      for (int i = 0; i < postArchives.Archive.Count; i++)
      {
        if (postArchives.Archive[i].Active == 'N')
        {
          _archiveRepository.DeleteArchiveById(postArchives.Archive[i].Id);
          postArchives.Archive.RemoveAt(i);
          i--;
        }
      }

      for (int i = 0; i < postArchives.Archive.Count; i++)
      {
        Archive archiveUpdate = new Archive();
        archiveUpdate.ArchiveType = archiveType;
        archiveUpdate.Description = postArchives.Post.Description;
        archives.Add(archiveUpdate);
      }

      postInformation.Restricted = postArchives.Post.Restricted;
      postInformation.Subject = postArchives.Post.Subject;
      postInformation.Description = postArchives.Post.Description;
      postInformation.CleanDescription = postInformation.Description;
      postInformation.Keywords = postArchives.Post.Keywords;
      postInformation.DepartmentsList = postArchives.DepartmentsList;

      _postRepository.UpdateArchivePost(postArchives.Post.Id, postInformation, archives);

      if (binaryData.Count != 0)
      {
        archives.Clear();

        for (int i = 0; i < binaryData.Count; i++)
        {
          Archive newArchive = new Archive();
          if (binaryData[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              binaryData[i].CopyTo(stream);
              newArchive.BinaryData = stream.ToArray();
              newArchive.Name = binaryData[i].FileName;
              newArchive.Active = 'S';
              newArchive.ArchiveType = archiveType;
              newArchive.Description = postInformation.Description;
              archives.Add(newArchive);
            }
          }
        }
        _postRepository.AddArchive(postArchives.Post.Id, archives);
      }
      return PartialView("ListAll");
    }

    public void RemovePost(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(HttpContext.Session.GetString("_DomainUsername"));
      var postCanBeDeleted = CheckIfPostCanBeDeleted(userAccount, post);

      if (postCanBeDeleted == true)
      {
        _postRepository.DeletePost(idPost);
      }
    }

    public bool CheckIfPostCanBeDeleted(UserAccount userAccount, Post post)
    {
      if (userAccount.Id == post.UserAccountId)
      {
        return true;
      }
      return false;
    }

    [HttpGet]
    public IActionResult QuestionDetails(int idPost)
    {
      var comments = new List<PostCommentViewModel>();
      var orderedPostComments = _postCommentRepository.GetPostCommentsByPostId(idPost);
      var post = _postRepository.GetPostById(idPost);
      var userAccount = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      var postsCount = _postRepository.GetPostsCountByUserId(userAccount.Id);

      for (int i = 0; i < orderedPostComments.Count; i++)
      {
        var postCommentViewModel = new PostCommentViewModel();
        var userAccountComment = _userAccountRepository.GetUserAccountById(orderedPostComments[i].UserAccountId);
        postCommentViewModel.Id = orderedPostComments[i].Id;
        postCommentViewModel.Description = orderedPostComments[i].Description;
        postCommentViewModel.Username = userAccountComment.Name;
        postCommentViewModel.Photo = userAccountComment.Photo;
        postCommentViewModel.UserId = orderedPostComments[i].UserAccountId;
        postCommentViewModel.DepartmentName = _departmentRepository.GetDepartmentNameById(orderedPostComments[i].DepartmentId);
        postCommentViewModel.ChangeDate = orderedPostComments[i].ChangeDate;
        postCommentViewModel.Files = _postCommentFileRepository.GetFilesByPostCommentId(orderedPostComments[i].Id).OrderBy(x => x.Name).ToList();
        comments.Add(postCommentViewModel);
      }
      ViewBag.Post = post;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;
      ViewBag.PostsCount = postsCount;
      ViewBag.PostComments = comments;

      return PartialView();
    }

    [HttpGet]
    public IActionResult ArchivePostDetails(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var orderedArchives = _archiveRepository.GetArchiveByPostId(idPost).OrderBy(a => a.Name).ToList();
      var userAccount = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      var postsCount = _postRepository.GetPostsCountByUserId(userAccount.Id);
      ViewBag.Post = post;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;
      ViewBag.Archives = orderedArchives;
      ViewBag.PostsCount = postsCount;

      return PartialView();
    }

    [HttpGet]
    public ActionResult ShowArchive(int idPost, int file)
    {
      var orderedArchives = _archiveRepository.GetArchiveByPostId(idPost).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + orderedArchives[file].Name);

      return File(orderedArchives[file].BinaryData, "application/pdf");
    }
    [HttpGet]
    public ActionResult ShowCommentFile(int idComment, int file)
    {
      var orderedArchives = _postCommentFileRepository.GetFilesByPostCommentId(idComment).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + orderedArchives[file].Name);

      return File(orderedArchives[file].BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult MakeComment(PostComment postComment, List<IFormFile> files)
    {
      if (!ModelState.IsValid)
      {
        return Json(-1);
      }

      postComment.UserAccountId = (int)HttpContext.Session.GetInt32("_UserAccountId");
      postComment.DepartmentId = (int)HttpContext.Session.GetInt32("_DepartmentId");
      postComment.CleanDescription = postComment.Description;

      if (files.Count > 0)
      {
        List<Archive> archiveList = new List<Archive>();

        for (int i = 0; i < files.Count; i++)
        {
          Archive archive = new Archive();
          archive.Name = files[i].FileName;
          archive.Description = postComment.Description;
          //archive.PostId = 0;
          if (files[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              files[i].CopyTo(stream);
              archive.BinaryData = stream.ToArray();
            }
          }
          archiveList.Add(archive);
        }
        _postCommentRepository.AddPostComment(postComment, archiveList);
      }
      else
      {
        _postCommentRepository.AddPostComment(postComment);
      }
      return Json("ListAll");
    }

    [HttpDelete]
    public IActionResult DeleteComment(int idComment)
    {
      var result = _postCommentRepository.DeletePostCommentById(idComment);
      return Json(result);
    }
  }
}
