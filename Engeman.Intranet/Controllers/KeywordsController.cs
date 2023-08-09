using Engeman.Intranet.Extensions;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq.Dynamic.Core;

namespace Engeman.Intranet.Controllers
{
  public class KeywordsController : RootController
  {
    private readonly IKeywordRepository _keywordRepository;

    public KeywordsController(IKeywordRepository keywordRepository)
    {
      _keywordRepository = keywordRepository;
    }

    [HttpGet]
    public IActionResult Grid()
    {
      var isModerator = HttpContext.Session.Get<bool>("_IsModerator");

      if (isModerator == false) return Redirect(Request.Host.ToString());

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");

      return PartialView("KeywordsGrid");
    }

    public JsonResult DataGrid(int rowCount, string searchPhrase, int current)
    {
      IQueryable<KeywordViewModel> keywords;
      IQueryable paginatedKeywords;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var orderedField = string.Format("{0} {1}", field, order);

      keywords = _keywordRepository.GetKeywordsGrid().AsQueryable();

      total = keywords.Count();

      if (!string.IsNullOrWhiteSpace(searchPhrase))
      {
        keywords = FilterKeywordsBySearchPhrase(keywords, searchPhrase);
        total = keywords.Count();
      }

      if (rowCount == -1) rowCount = total;

      paginatedKeywords = OrderedKeywords(keywords, orderedField, current, rowCount);

      return Json(new { rows = paginatedKeywords, current, rowCount, total });
    }

    public IQueryable<KeywordViewModel> FilterKeywordsBySearchPhrase(IQueryable<KeywordViewModel> keywords, string searchPhrase)
    {
      return keywords.Where("description.Contains(@0, StringComparison.OrdinalIgnoreCase)", searchPhrase);
    }

    public IQueryable OrderedKeywords(IQueryable<KeywordViewModel> keywords, string orderedField, int current, int rowCount)
    {
      return keywords.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
    }

    [HttpGet]
    public IActionResult NewKeyword()
    {
      return PartialView("NewKeywordForm");
    }

    [HttpPost]
    public JsonResult NewKeyword(IFormCollection formData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;
      var newKeyword = new KeywordViewModel
      {
        Description = formData["description"]
      };

      try { _keywordRepository.Add(newKeyword, sessionUsername); }
      catch (SqlException ex)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        if (ex.Number == 2627)
        {
          messageAux = "A palavra-chave já existe na base de dados.";
        }
        else
        {
          messageAux = ex.Message;
        }
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpGet]
    public IActionResult EditKeyword(int keywordId)
    {
      if (!HttpContext.Request.IsAjaxOrFetch("GET")) return Redirect(Request.Host.ToString());

      var keywordAux = _keywordRepository.GetById(keywordId);
      var keyword = new KeywordViewModel
      {
        Id = keywordAux.Id,
        Description = keywordAux.Description,
        ChangeDate = keywordAux.ChangeDate.ToString()
      };
      return PartialView("EditKeywordForm", keyword);
    }

    [HttpPut]
    public JsonResult UpdateKeyword(IFormCollection formData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;
      var editedKeyword = new Keyword
      {
        Id = Convert.ToInt32(formData["id"]),
        Description = formData["description"],
      };

      try { _keywordRepository.Update(editedKeyword.Id, editedKeyword, sessionUsername); }
      catch (SqlException ex)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        if (ex.Number == 2627)
        {
          messageAux = "A palavra-chave já existe na base de dados.";
        }
        else
        {
          messageAux = ex.Message;
        }
      }
      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpDelete]
    public JsonResult DeleteKeyword(int keywordId)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;

      try { _keywordRepository.Delete(keywordId, sessionUsername); }
      catch (SqlException sqlEx)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        messageAux = sqlEx.Message;
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpGet]
    public JsonResult GetKeywordList()
    {
      var keywords = _keywordRepository.GetIdAndName();
      return Json(keywords);
    }
  }
}
