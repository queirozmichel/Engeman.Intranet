using Microsoft.AspNetCore.Mvc;
using System;

namespace Engeman.Intranet.ViewComponents
{
  public class WangEditorViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      String selectorId = "";

      Random res = new Random();
      String str = "abcdefghijklmnopqrstuvwxyz";
      int size = 5;

      for (int i = 0; i < size; i++)
      {
        int x = res.Next(str.Length);
        selectorId = selectorId + str[x];
      }
      return View("Default", selectorId);
    }
  }
}