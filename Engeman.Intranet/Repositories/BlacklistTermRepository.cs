using Engeman.Intranet.Library;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public class BlacklistTermRepository : IBlacklistTermRepository
  {
    public List<BlacklistTerm> Get()
    {
      var BlackList = new List<BlacklistTerm>();
      var query = $"SELECT * FROM BLACKLISTTERM";

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
        BlackList.Add(term);
      }
      return BlackList;
    }

    public List<string> GetTerms()
    {
      var blacklist = new List<string>();
      string term;
      var query = $"SELECT DESCRIPTION FROM BLACKLISTTERM";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        term = result.Rows[i]["description"].ToString();
        blacklist.Add(term);
      }
      return blacklist;
    }
  }
}
