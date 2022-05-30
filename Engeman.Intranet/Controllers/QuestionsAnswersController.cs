using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class QuestionsAnswersController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
