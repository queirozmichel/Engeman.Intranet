using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents.Charts
{
  public class PostsCommentsChartViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View("/Views/Shared/Components/Charts/PostsCommentsChart/Default.cshtml");
    }
  }
}