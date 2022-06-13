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
  public class QuestionsAnswersController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;

    public QuestionsAnswersController(IUserAccountRepository userAccountRepository)
    {
      _userAccountRepository = userAccountRepository;
    }
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost]
    public JsonResult GetDataToGrid(string searchPhrase, int current = 1, int rowCount = 5)
    {
      int total = 0;
      IQueryable paginatedUsers;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var allUsers = _userAccountRepository.GetAllUserAccount().AsQueryable();
      string orderedField = String.Format("{0} {1}", field, order);
      total = allUsers.Count();

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        int id = 0;
        int.TryParse(searchPhrase, out id);

        allUsers = allUsers.Where("name.Contains(@0) OR domainAccount.Contains(@0) OR " +
          "email.Contains(@0) OR changeDate.Contains(@0) OR id == (@1)", searchPhrase, id);
      }

      if (orderedField.Contains("changeDate asc"))
      {
        paginatedUsers = allUsers.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        total = allUsers.Count();
        return Json(new { rows = paginatedUsers, current, rowCount, total });
      }
      else if (orderedField.Contains("changeDate desc"))
      {
        paginatedUsers = allUsers.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
        total = allUsers.Count();
        return Json(new { rows = paginatedUsers, current, rowCount, total });
      }

      total = allUsers.Count();
      paginatedUsers = allUsers.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);

      return Json(new { rows = paginatedUsers, current, rowCount, total });
    }
  }
}
