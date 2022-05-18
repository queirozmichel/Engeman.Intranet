using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers {
    public class QuestionsAnswersController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
