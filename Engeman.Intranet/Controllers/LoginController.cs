using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  public class LoginController : Controller
  {
    public IActionResult Index()
    {
      return PartialView();
    }
  }
}
