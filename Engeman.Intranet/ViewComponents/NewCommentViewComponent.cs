using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class NewCommentViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
