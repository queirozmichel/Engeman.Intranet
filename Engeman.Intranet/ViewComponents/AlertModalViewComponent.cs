using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class AlertModalViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke(string text)
    {
      if (text != null)
      {
        ViewBag.Title = text.Substring(0, text.IndexOf("/"));
        ViewBag.Body = text.Substring(text.IndexOf("/") + 1);
      }

      return View("Default");
    }
  }
}
