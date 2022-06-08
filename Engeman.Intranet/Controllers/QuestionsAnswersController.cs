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
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var profilesInfo = _userAccountRepository.GetAllUserProfiles().AsQueryable();
      string orderedField = String.Format("{0} {1}", field, order);

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        int id = 0;
        int.TryParse(searchPhrase, out id);        

        profilesInfo = profilesInfo.Where("name.Contains(@0) OR domainAccount.Contains(@0) OR " +
          "email.Contains(@0) OR changeDate.Contains(@0) OR id == (@1)", searchPhrase, id);
      }

      int total = profilesInfo.Count();
      var profilesPaginated = profilesInfo.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);

      return Json(new { rows = profilesPaginated, current, rowCount, total });
    }
  }
}
