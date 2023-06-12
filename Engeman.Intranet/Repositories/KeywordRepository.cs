using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class KeywordRepository : IKeywordRepository
  {
    public List<KeywordViewModel> GetKeywordsGrid()
    {
      var keywordList = new List<KeywordViewModel>();
      var query = $"SELECT ID, DESCRIPTION, CHANGE_DATE FROM KEYWORD";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var keyword = new KeywordViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Description = result.Rows[i]["description"].ToString(),
          ChangeDate = result.Rows[i]["change_date"].ToString()
        };
        keywordList.Add(keyword);
      }
      return keywordList;
    }

    public List<Keyword> Get()
    {
      var query = $"SELECT * FROM KEYWORD";
      var keywordList = new List<Keyword>();

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var keyword = new Keyword
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Description = result.Rows[i]["description"].ToString(),
          ChangeDate = (DateTime)result.Rows[i]["change_Date"]
        };
        keywordList.Add(keyword);
      }
      return keywordList;
    }

    public Keyword GetById(int id)
    {
      var query = $"SELECT * FROM KEYWORD WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0].Rows[0];

      var keyword = new Keyword
      {
        Id = Convert.ToInt32(result["id"]),
        Description = result["description"].ToString(),
        ChangeDate = (DateTime)result["change_Date"]
      };
      return keyword;
    }

    public void Add(KeywordViewModel keyword, string currentUsername = null)
    {
      var query = $"INSERT INTO KEYWORD (DESCRIPTION) OUTPUT INSERTED.ID VALUES ('{keyword.Description}')";

      using StaticQuery sq = new();
      var outputKeywordId = sq.GetDataToInt(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('I', "KEY", outputKeywordId, "KEYWORD", currentUsername);
      }
    }

    public void Update(int id, Keyword keyword, string currentUsername)
    {
      var query = $"UPDATE KEYWORD SET DESCRIPTION = '{keyword.Description}'WHERE ID = '{keyword.Id}'";

      using StaticQuery sq = new StaticQuery();
      sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('U', "KEY", id, "KEYWORD", currentUsername);
      }
    }

    public void Delete(int id, string currentUsername = null)
    {
      string query = $"DELETE FROM KEYWORD WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('D', "KEY", id, "KEYWORD", currentUsername);
      }
    }

    public List<KeywordComponentViewModel> GetIdAndName()
    {
      var query = $"SELECT Id, Description FROM KEYWORD";
      var keywordList = new List<KeywordComponentViewModel>();

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var keyword = new KeywordComponentViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Name = result.Rows[i]["description"].ToString()
        };
        keywordList.Add(keyword);
      }
      return keywordList;
    }
  }
}
