using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers {
    public class DashboardController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
