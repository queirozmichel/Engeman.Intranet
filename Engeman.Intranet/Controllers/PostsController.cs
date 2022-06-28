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

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class PostsController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository, IDepartmentRepository departmentRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _departmentRepository = departmentRepository;
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

    [HttpPost]
    public ActionResult InsertArchive(PostArchiveDto postArchiveDto, List<IFormFile> binaryData)
    {
      bool statusPost = true;
      bool statusArchive = false;
      bool status = false;
      
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
        archive.Active = 'S';
        archive.ArchiveType = postArchiveDto.Archive.ArchiveType;
        archive.Name = binaryData[0].FileName;
        archive.Description = postArchiveDto.Post.Description;
        archive.PostId = 0;
        statusArchive = true;
      }

      if (statusArchive == true & statusPost == true)
      {
        _postRepository.AddArchive(askQuestionDto,archive);
        status = true;
      }

      return Json(status);
    }
  }
}
