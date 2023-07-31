using Engeman.Intranet.Extensions;
using Engeman.Intranet.Helpers;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace Engeman.Intranet.Controllers
{
  public class BlacklistTermsController : RootController
  {
    private readonly IBlacklistTermRepository _blacklistTermRepository;

    public BlacklistTermsController(IBlacklistTermRepository blacklistTermRepository)
    {
      _blacklistTermRepository = blacklistTermRepository;
    }

    [HttpGet]
    public IActionResult Grid()
    {
      var isModerator = HttpContext.Session.Get<bool>("_IsModerator");

      if (isModerator == false) return Redirect(Request.Host.ToString());

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView("BlacklistTermsGrid");
    }

    public JsonResult DataGrid(int rowCount, string searchPhrase, int current)
    {
      IQueryable<BlacklistTermViewModel> terms;
      IQueryable paginatedBlacklistTerms;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var orderedField = string.Format("{0} {1}", field, order);

      terms = _blacklistTermRepository.GetBlacklistTermsGrid().AsQueryable();

      total = terms.Count();

      if (!string.IsNullOrWhiteSpace(searchPhrase))
      {
        terms = FilterTermsBySearchPhrase(terms, searchPhrase);
        total = terms.Count();
      }

      if (rowCount == -1) rowCount = total;

      paginatedBlacklistTerms = OrderedTerms(terms, orderedField, current, rowCount);

      return Json(new { rows = paginatedBlacklistTerms, current, rowCount, total });
    }

    public IQueryable<BlacklistTermViewModel> FilterTermsBySearchPhrase(IQueryable<BlacklistTermViewModel> terms, string searchPhrase)
    {
      return terms.Where("description.Contains(@0, StringComparison.OrdinalIgnoreCase)", searchPhrase);
    }

    public IQueryable OrderedTerms(IQueryable<BlacklistTermViewModel> terms, string orderedField, int current, int rowCount)
    {
      return terms.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
    }

    [HttpGet]
    public IActionResult NewTerm()
    {
      return PartialView("NewTermForm");
    }

    [HttpPost]
    public JsonResult NewTerm(IFormCollection formData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;

      try { _blacklistTermRepository.Add(formData["description"].ToString().ToLower(), sessionUsername); }
      catch (SqlException ex)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        if (ex.Number == 2627)
        {
          messageAux = "O termo já existe na base de dados.";
        }
        else
        {
          messageAux = ex.Message;
        }
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpGet]
    public IActionResult EditTerm(int termId)
    {
      if (!HttpContext.Request.IsAjax("GET")) return Redirect(Request.Host.ToString());

      var termAux = _blacklistTermRepository.GetById(termId);
      var term = new BlacklistTermViewModel
      {
        Id = termAux.Id,
        Description = termAux.Description,
        ChangeDate = termAux.ChangeDate.ToString()
      };
      return PartialView("EditTermForm", term);
    }

    [HttpPut]
    public JsonResult UpdateTerm(IFormCollection formData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;   

      try { _blacklistTermRepository.Update(Convert.ToInt32(formData["id"]), formData["description"].ToString().ToLower(), sessionUsername); }
      catch (SqlException ex)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        if (ex.Number == 2627)
        {
          messageAux = "O termo já existe na base de dados.";
        }
        else
        {
          messageAux = ex.Message;
        }
      }
      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpDelete]
    public JsonResult DeleteTerm(int termId)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;

      try { _blacklistTermRepository.Delete(termId, sessionUsername); }
      catch (SqlException sqlEx)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        messageAux = sqlEx.Message;
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    /// <summary>
    /// Testa se a string contém algum termo existente na tabela de termos proibidos.
    /// Se sim, cria um array de string com as palavras que foram encontradas nos campos("keys") do respectivo formulário para ser mostrada no modal de alerta.
    /// </summary>
    /// <param name="formData">Entradas do formulário com seus respectivos valores</param>
    [HttpPost]
    public JsonResult BlacklistTest(IFormCollection formData)
    {
      var blacklist = _blacklistTermRepository.Get();
      var keys = formData.Keys.Where(x => x.Equals("subject") || x.Equals("description") || x.Equals("comment.description") || x.Equals("name") || x.Equals("username")).ToList();
      string text = string.Empty;
      string regexPattern = "(?i)";
      int count = 0;
      string termsFounded = string.Empty;

      foreach (var key in keys)
      {
        if ((key == "description" || key == "comment.description"))
        {
          text = GlobalFunctions.HTMLToTextConvert(formData[key]);
        }
        else
        {
          text = formData[key];
        }

        text = GlobalFunctions.CleanText(text);

        if (blacklist.Count == 0)
        {
          return Json(new { occurrences = count });
        }
        else
        {
          for (int i = 0; i < blacklist.Count; i++)
          {
            regexPattern = "(?i)" + "(\\b" + blacklist[i].Description + "\\b)";
            if (Regex.Matches(text, regexPattern).Count != 0)
            {
              termsFounded = termsFounded + "\"" + blacklist[i].Description + "\" ";
              count++;
            }
          }
        }
      }
      string[] termsFoundedArray = (termsFounded.Split(" ").Distinct().ToArray())[..^1];
      return Json(new { occurrences = count, termsFounded = termsFoundedArray });
    }
  }
}
