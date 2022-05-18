using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers {
    public class UserProfileController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
