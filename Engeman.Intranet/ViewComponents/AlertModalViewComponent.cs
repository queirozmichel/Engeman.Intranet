using Microsoft.AspNetCore.Mvc;
using System;

namespace Engeman.Intranet.ViewComponents
{
  public class AlertModalViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke(string selector)
    {
      if (selector == null)
      {
        Random res = new Random();
        String str = "abcdefghijklmnopqrstuvwxyz";
        int size = 5;

        for (int i = 0; i < size; i++)
        {
          int x = res.Next(str.Length);
          selector = selector + str[x];
        }
      }
      return View("Default", selector);
    }
  }
}
