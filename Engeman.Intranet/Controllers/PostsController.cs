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

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class PostsController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;

    public PostsController(IUserAccountRepository userAccountRepository, IPostRepository postRepository)
    {
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
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

      if (orderedField.Contains("changeDate asc"))
      {
        paginatedPosts = allPosts.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        total = allPosts.Count();
        return Json(new { rows = paginatedPosts, current, rowCount, total });
      }
      else if (orderedField.Contains("changeDate desc"))
      {
        paginatedPosts = allPosts.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        total = allPosts.Count();
        return Json(new { rows = paginatedPosts, current, rowCount, total });
      }

      total = allPosts.Count();
      paginatedPosts = allPosts.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);

      return Json(new { rows = paginatedPosts, current, rowCount, total });
    }

    public IActionResult SaveQuestion(AskQuestionDto askQuestionDto)
    {

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccount(sessionDomainUsername);
      askQuestionDto.UserAccountId = userAccount.Id;
      askQuestionDto.DepartmentId = userAccount.DepartmentId;
      askQuestionDto.PostType = 'Q';
      askQuestionDto.Active = 'S';
      askQuestionDto.CleanDescription = askQuestionDto.Description;
      askQuestionDto.DomainAccount = sessionDomainUsername;

      _postRepository.InsertQuestion(askQuestionDto);

      return View("AskQuestion");
    }
  }
}
