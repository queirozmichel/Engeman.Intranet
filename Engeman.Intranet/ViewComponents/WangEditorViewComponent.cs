using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class WangEditorViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke(string selectorId)
    {
      return View("Default", selectorId);
    }
  }
}