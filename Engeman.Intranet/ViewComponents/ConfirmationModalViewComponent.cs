using Microsoft.AspNetCore.Mvc;
using System;

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
