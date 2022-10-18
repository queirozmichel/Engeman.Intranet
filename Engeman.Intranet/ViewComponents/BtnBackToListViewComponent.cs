using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class BtnBackToListViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
