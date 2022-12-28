using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public FileStreamResult GenerateLog()
    {
      var log = _logRepository.GetFormatted();
      byte[] byteLogArray = log.SelectMany(s => Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
      var stream = new MemoryStream(byteLogArray);

      return File(stream, "text/plain", "arquivo_de_log.txt");
    }
  }
}