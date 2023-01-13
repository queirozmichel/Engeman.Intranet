using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class LogsController : Controller
  {
    private readonly ILogRepository _logRepository;

    public LogsController(ILogRepository logRepository)
    {
      _logRepository = logRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Grid()
    {
      var isModerator = Convert.ToBoolean(HttpContext.Session.GetInt32("_Moderator"));
      if (isModerator == false) return Redirect(Request.Host.ToString());

      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      ViewBag.IsAjaxCall = isAjaxCall;
      return PartialView("LogsGrid");
    }

    public JsonResult DataGrid(string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<LogGridViewModel> logs = null;
      IQueryable paginatedLogs;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      string orderedField = String.Format("{0} {1}", field, order);

      logs =  _logRepository.GetLogsGrid().AsQueryable(); ;
      total = logs.Count();

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        logs = FilterLogsBySearchPhrase(logs, searchPhrase);
        total = logs.Count();
      }

      if (rowCount == -1)
      {
        rowCount = total;
      }

      paginatedLogs = OrderedLogs(logs, orderedField, current, rowCount);

      return Json(new { rows = paginatedLogs, current, rowCount, total });
    }

    public IQueryable<LogGridViewModel> FilterLogsBySearchPhrase(IQueryable<LogGridViewModel> logs, string searchPhrase)
    {
      int id = 0;
      int.TryParse(searchPhrase, out id);
      logs = logs.Where("username.Contains(@0, StringComparison.OrdinalIgnoreCase) OR operation.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR description.Contains(@0, StringComparison.OrdinalIgnoreCase) OR referenceTable.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR changeDate.Contains(@0) OR id == (@1) OR referenceId == (@1)", searchPhrase, id);
      return logs;
    }

    public IQueryable OrderedLogs(IQueryable<LogGridViewModel> logs, string orderedField, int current, int rowCount)
    {
      IQueryable aux;
      if (orderedField.Contains("changeDate asc"))
      {
        return aux = logs.OrderBy(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      }
      else if (orderedField.Contains("changeDate desc"))
      {
        return aux = logs.OrderByDescending(x => Convert.ToDateTime(x.ChangeDate)).Skip((current - 1) * rowCount).Take(rowCount);
      }
      else
      {
        return aux = logs.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
      }
    }

    //[HttpGet]
    //public FileStreamResult GenerateLog()
    //{
    //  var log = _logRepository.GetFormatted();
    //  byte[] byteLogArray = log.SelectMany(s => Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
    //  var stream = new MemoryStream(byteLogArray);

    //  return File(stream, "text/plain", "arquivo_de_log.txt");
    //}
  }
}