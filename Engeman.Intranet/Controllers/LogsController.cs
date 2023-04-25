using Engeman.Intranet.Extensions;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class LogsController : Controller
  {
    private readonly ILogRepository _logRepository;
    private IUserAccountRepository _userAccountRepository;

    public LogsController(ILogRepository logRepository, IUserAccountRepository userAccountRepository)
    {
      _logRepository = logRepository;
      _userAccountRepository = userAccountRepository;
    }

    [HttpGet]
    public IActionResult Grid(bool filterByUsername, int userId)
    {
      if (HttpContext.Session.Get<bool>("_IsModerator") == false) return Redirect(Request.Host.ToString());

      if (filterByUsername == true)
      {
        ViewBag.FilterByUsername = filterByUsername;
        try { ViewBag.Username = _userAccountRepository.GetUsernameById(userId); } catch (Exception) { }
      }

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView("LogsGrid");
    }

    public JsonResult DataGrid(string username, string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<LogGridViewModel> logs = null;
      IQueryable paginatedLogs;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      string orderedField = string.Format("{0} {1}", field, order);

      if (username != null) logs = _logRepository.GetLogsGrid().AsQueryable().Where("username == @0", username);
      else logs = _logRepository.GetLogsGrid().AsQueryable();

      total = logs.Count();

      if (!string.IsNullOrWhiteSpace(searchPhrase))
      {
        logs = FilterLogsBySearchPhrase(logs, searchPhrase);
        total = logs.Count();
      }

      if (rowCount == -1) rowCount = total;

      paginatedLogs = OrderedLogs(logs, orderedField, current, rowCount);

      return Json(new { rows = paginatedLogs, current, rowCount, total });
    }

    public IQueryable<LogGridViewModel> FilterLogsBySearchPhrase(IQueryable<LogGridViewModel> logs, string searchPhrase)
    {
      int.TryParse(searchPhrase, out int id);

      return logs.Where("id == (@1) OR operation.Contains(@0, StringComparison.OrdinalIgnoreCase) OR registryType.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR registryId == (@1) OR registryTable.Contains(@0, StringComparison.OrdinalIgnoreCase) OR username.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR changeDate.Contains(@0)", searchPhrase, id);
    }

    public IQueryable OrderedLogs(IQueryable<LogGridViewModel> logs, string orderedField, int current, int rowCount)
    {
      if (orderedField.Contains("changeDate asc")) return logs.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      else if (orderedField.Contains("changeDate desc")) return logs.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      else return logs.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
    }
  }
}