using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class BlacklistTermRepository : IBlacklistTermRepository
  {
    public List<BlacklistTermViewModel> GetBlacklistTermsGrid()
    {
      var blacklist = new List<BlacklistTermViewModel>();
      var query = $"SELECT ID, DESCRIPTION, CHANGE_DATE FROM BLACKLISTTERM";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var term = new BlacklistTermViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Description = result.Rows[i]["description"].ToString(),
          ChangeDate = result.Rows[i]["change_date"].ToString()
        };
        blacklist.Add(term);
      }
      return blacklist;
    }

    public List<BlacklistTerm> Get()
    {
      var query = $"SELECT * FROM BLACKLISTTERM";
      var blackList = new List<BlacklistTerm>();

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var term = new BlacklistTerm
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Description = result.Rows[i]["description"].ToString(),
          ChangeDate = (DateTime)result.Rows[i]["change_Date"]
        };
        blackList.Add(term);
      }
      return blackList;
    }

    public BlacklistTerm GetById(int id)
    {
      var query = $"SELECT * FROM BLACKLISTTERM WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0].Rows[0];

      var term = new BlacklistTerm
      {
        Id = Convert.ToInt32(result["id"]),
        Description = result["description"].ToString(),
        ChangeDate = (DateTime)result["change_Date"]
      };
      return term;
    }

    public void Add(BlacklistTermViewModel term, string currentUsername)
    {
      var query = $"INSERT INTO BLACKLISTTERM (DESCRIPTION) OUTPUT INSERTED.ID VALUES ('{term.Description}')";

      using StaticQuery sq = new();
      var outputTermId = sq.GetDataToInt(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('I', "TER", outputTermId, "BLACKLISTTERM", currentUsername);
      }
    }

    public void Update(int id, BlacklistTerm term, string currentUsername)
    {
      var query = $"UPDATE BLACKLISTTERM SET DESCRIPTION = '{term.Description}'WHERE ID = '{term.Id}'";

      using StaticQuery sq = new StaticQuery();
      sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('U', "TER", id, "BLACKLISTTERM", currentUsername);
      }
    }

    public void Delete(int id, string currentUsername = null)
    {
      string query = $"DELETE FROM BLACKLISTTERM WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('D', "TER", id, "BLACKLISTTERM", currentUsername);
      }
    }
  }
}