using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  public class PostKeywordsController : RootController
  {
    private readonly IKeywordRepository _keywordRepository;
    private readonly IPostKeywordRepository _postKeywordRepository;

    public PostKeywordsController(IKeywordRepository keywordRepository, IPostKeywordRepository postKeywordRepository)
    {
      _keywordRepository = keywordRepository;
      _postKeywordRepository = postKeywordRepository;
    }

    [HttpGet]
    public JsonResult GetKeywordList(int postId)
    {
      var keywords = _keywordRepository.GetIdAndName();
      var postKeywords = _postKeywordRepository.GetIdAndNameByPostId(postId);
      return Json(new { keywords, postKeywords });
    }
  }
}
