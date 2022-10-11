using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class CommentListViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
