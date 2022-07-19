using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web;
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

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository, IDepartmentRepository departmentRepository, IArchiveRepository archiveRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
      _archiveRepository = archiveRepository;
    }
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult ListAll()
    {
      return View();
    }

    public IActionResult AskQuestion()
    {
      return View();
    }
    public IActionResult BackToList()
    {
      return PartialView("ListAll");
    }

    [HttpGet]
    public IActionResult InsertArchive()
    {
      return View();
    }

    [HttpPost]
    public JsonResult GetDataGrid(string searchPhrase, int current = 1, int rowCount = 5)
    {
      int total = 0;
      IQueryable paginatedPosts;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var allPosts = _postRepository.GetAllPosts().AsQueryable();
      string orderedField = String.Format("{0} {1}", field, order);
      total = allPosts.Count();

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        int id = 0;
        int.TryParse(searchPhrase, out id);

        allPosts = allPosts.Where("userAccountName.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
          "departmentDescription.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
          "subject.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
          "cleanDescription.Contains(@0, StringComparison.OrdinalIgnoreCase) OR " +
          "changeDate.Contains(@0) OR id == (@1)", searchPhrase, id);
      }

      total = allPosts.Count();
      if (rowCount == -1)
      {
        rowCount = total;
      }

      if (orderedField.Contains("changeDate asc"))
      {
        paginatedPosts = allPosts.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        return Json(new { rows = paginatedPosts, current, rowCount, total });
      }
      else if (orderedField.Contains("changeDate desc"))
      {
        paginatedPosts = allPosts.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        return Json(new { rows = paginatedPosts, current, rowCount, total });
      }

      paginatedPosts = allPosts.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);

      return Json(new { rows = paginatedPosts, current, rowCount, total });
    }

    [HttpPost]
    public IActionResult SaveQuestion(AskQuestionDto askQuestionDto)
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

      return View("AskQuestion");
    }

    public IActionResult QuestionEdit(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      return PartialView(post);
    }

    public IActionResult ArchivePostEdit(int idPost)
    {
      PostArchiveDto postArchiveDto = new PostArchiveDto();

      var post = _postRepository.GetPostById(idPost);
      var archive = _archiveRepository.GetArchiveByPostId(idPost);

      postArchiveDto.Post = post;
      postArchiveDto.Archive = archive;


      return PartialView(postArchiveDto);
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
    public ActionResult UpdateArchive(PostArchiveDto postArchiveDto, List<IFormFile> binaryData)
    {
      if (!ModelState.IsValid)
      {
        return Json(0);
      }
      bool statusArchive = false;

      AskQuestionDto askQuestionDto = new AskQuestionDto();
      Archive archive = new Archive();

      askQuestionDto.Restricted = postArchiveDto.Post.Restricted;
      askQuestionDto.Subject = postArchiveDto.Post.Subject;
      askQuestionDto.Description = postArchiveDto.Post.Description;
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.Keywords = postArchiveDto.Post.Keywords;
      archive.ArchiveType = postArchiveDto.Archive.ArchiveType;
      archive.Description = postArchiveDto.Post.Description;

      if (binaryData.Count != 0)
      {
        foreach (var item in binaryData)
        {
          if (item.Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              item.CopyTo(stream);
              archive.BinaryData = stream.ToArray();
            }
          }
        }
        archive.Name = binaryData[0].FileName;
        statusArchive = true;
      }
      else
      {
        var aux = _archiveRepository.GetArchiveByPostId(postArchiveDto.Post.Id);
        archive.BinaryData = aux.BinaryData;
        archive.Name = aux.Name;
        statusArchive = true;
      }

      if (statusArchive == true)
      {
        _postRepository.UpdateArchivePost(postArchiveDto.Post.Id, askQuestionDto, archive);
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

    public IActionResult QuestionDetails(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var userAccount = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      ViewBag.Post = post;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;

      return PartialView();
    }

    public FileContentResult DownloadArchive(int idPost)
    {
      Archive archive = new Archive();
      archive = _archiveRepository.GetArchiveByPostId(idPost);
      byte[] fileBytes = archive.BinaryData;
      var response = new FileContentResult(fileBytes, "application/octet-stream");
      response.FileDownloadName = archive.Name;
      return response;
    }

    public IActionResult ArchivePostDetails(int idPost)
    {
      var post = _postRepository.GetPostById(idPost);
      var archive = _archiveRepository.GetArchiveByPostId(idPost);
      var userAccount = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      ViewBag.Post = post;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;
      ViewBag.Archive = archive;

      return PartialView(archive);
    }
    [HttpGet]
    public ActionResult ShowArchive(int idPost)
    {
      var aux = _archiveRepository.GetArchiveByPostId(idPost);
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + aux.Name);
      return File(aux.BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult InsertArchive(PostArchiveDto postArchiveDto, List<IFormFile> binaryData)
    {
      if (!ModelState.IsValid || binaryData.Count == 0)
      {
        return Json(0);
      }

      AskQuestionDto askQuestionDto = new AskQuestionDto();
      Archive archive = new Archive();

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      askQuestionDto.Active = 'S';
      askQuestionDto.Restricted = postArchiveDto.Post.Restricted;
      askQuestionDto.Subject = postArchiveDto.Post.Subject;
      askQuestionDto.Description = postArchiveDto.Post.Description;
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.Keywords = postArchiveDto.Post.Keywords;
      askQuestionDto.UserAccountId = userAccount.Id;
      askQuestionDto.DomainAccount = sessionDomainUsername;
      askQuestionDto.DepartmentId = userAccount.DepartmentId;
      askQuestionDto.PostType = 'A';
      archive.Active = 'S';
      archive.ArchiveType = postArchiveDto.Archive.ArchiveType;
      archive.Name = binaryData[0].FileName;
      archive.Description = postArchiveDto.Post.Description;
      archive.PostId = 0;

      foreach (var item in binaryData)
      {
        if (item.Length > 0)
        {
          using (var stream = new MemoryStream())
          {
            item.CopyTo(stream);
            archive.BinaryData = stream.ToArray();
          }
        }
      }

      _postRepository.AddArchive(askQuestionDto, archive);

      return View("InsertArchive");
    }
  }
}
