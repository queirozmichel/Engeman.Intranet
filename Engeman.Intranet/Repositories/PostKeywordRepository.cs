using Engeman.Intranet.Library;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class PostKeywordRepository : IPostKeywordRepository
  {
    public string[] GetKeywordsByPostId(int id)
    {
      var query = $"SELECT ID, POST_ID, KEYWORD_ID, KEYWORD FROM POSTKEYWORD WHERE POST_ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];
      string[] keywords = new string[result.Rows.Count];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        keywords[i] = result.Rows[i]["keyword"].ToString();
      }
      return keywords;
    }

    public List<KeywordComponentViewModel> GetIdAndNameByPostId(int id)
    {
      var query = $"SELECT KEYWORD_ID, KEYWORD FROM POSTKEYWORD WHERE POST_ID = {id}";
      var keywordList = new List<KeywordComponentViewModel>();

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var keyword = new KeywordComponentViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["keyword_id"]),
          Name = result.Rows[i]["keyword"].ToString()
        };
        keywordList.Add(keyword);
      }
      return keywordList;
    }
  }
}