﻿using Engeman.Intranet.Models;
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
using System.Collections;

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
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      return View();
    }
    public IActionResult BackToList()
    {
      return PartialView("ListAll");
    }

    [HttpGet]
    public IActionResult InsertArchive()
    {
      ViewBag.Departments = _departmentRepository.GetAllDepartments();
      return View();
    }

    [HttpPost]
    public JsonResult GetDataGrid(string filter, int rowCount, string searchPhrase, int current)
    {
      int total = 0;
      IQueryable paginatedPosts;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var departmentIdSession = HttpContext.Session.GetInt32("_DepartmentId");
      var userIdSession = HttpContext.Session.GetInt32("_Id");
      var allPosts = _postRepository.GetPostsByRestriction((int)departmentIdSession, (int)userIdSession).AsQueryable();

      if (filter == "manual")
      {
        allPosts = allPosts.Where("archiveType == (@0)", "M");
      }
      else if (filter == "document")
      {
        allPosts = allPosts.Where("archiveType == (@0)", "D");
      }
      else if (filter == "question")
      {
        allPosts = allPosts.Where(x => x.PostType == 'Q');
      }
      else if (filter == "my")
      {
        allPosts = allPosts.Where(x => x.UserAccountId == userIdSession);
      }
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
      //var archives = _archiveRepository.GetArchiveByPostId(idPost);
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
      var post = _postRepository.GetPostById(idPost);
      var userAccount = _userAccountRepository.GetUserAccountById(post.UserAccountId);
      var department = _departmentRepository.GetDepartmentById(userAccount.DepartmentId);
      var postsCount = _postRepository.GetPostsCountByUserId(userAccount.Id);
      ViewBag.Post = post;
      ViewBag.UserAccount = userAccount;
      ViewBag.Department = department;
      ViewBag.PostsCount = postsCount;

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

    [HttpPost]
    public IActionResult AddArchive(PostArchiveDto postArchiveDto, List<IFormFile> binaryData)
    {
      if (!ModelState.IsValid || binaryData.Count == 0)
      {
        return Json(0);
      }

      AskQuestionDto askQuestionDto = new AskQuestionDto();
      List<Archive> archiveList = new List<Archive>();

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);
      askQuestionDto.Active = 'S';
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
        archive.Active = 'S';
        archive.ArchiveType = postArchiveDto.Archive.ArchiveType;
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

      return View("InsertArchive");
    }
  }
}
