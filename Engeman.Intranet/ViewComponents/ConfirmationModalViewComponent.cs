using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class ConfirmationModalViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {   
      return View("Default");
    }
  }
}
