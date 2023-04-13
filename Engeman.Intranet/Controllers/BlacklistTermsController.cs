using Engeman.Intranet.Helpers;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Engeman.Intranet.Controllers
{
  public class BlacklistTermsController : Controller
  {
    private readonly IBlacklistTermRepository _blacklistTermRepository;

    public BlacklistTermsController(IBlacklistTermRepository blacklistTermRepository)
    {
      _blacklistTermRepository = blacklistTermRepository;
    }

    public IActionResult Index()
    {
      return View();
    }

    /// <summary>
    /// Testa se a string contém algum termo existente na tabela de termos proibidos.
    /// </summary>
    /// <param name="formData">Entradas do formulário com seus respectivos valores</param>
    [HttpPost]
    public JsonResult BlacklistTest(IFormCollection formData)
    {
      var blacklist = _blacklistTermRepository.GetTerms();
      var keys = formData.Keys.Where(x => x.Equals("subject") || x.Equals("keywords") || x.Equals("description") || x.Equals("comment.description") || x.Equals("name")).ToList();
      string text = string.Empty;
      string regexPattern = "(?i)";

      foreach (var key in keys)
      {
        if (key == "Description")
        {
          text += GlobalFunctions.HTMLToTextConvert(formData[key]) + " ";
          continue;
        }
        text += formData[key] + " ";
      }

      text = GlobalFunctions.CleanText(text);

      for (int i = 0; i < blacklist.Count; i++)
      {
        if (i + 1 == blacklist.Count)
        {
          regexPattern += "(\\b" + blacklist[i] + "\\b)";
        }
        else
        {
          regexPattern += "(\\b" + blacklist[i] + "\\b)|";
        }
      }

      return Json(new { occurrences = Regex.Matches(text, regexPattern).Count });
    }
  }
}
