using Engeman.Intranet.Library;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public class ForbiddenWordRepository : IForbiddenWordRepository
  {
    public List<ForbiddenWord> Get()
    {
      var forbiddenWords = new List<ForbiddenWord>();
      var query = $"SELECT * FROM FORBIDDENWORD";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var forbiddenWord = new ForbiddenWord
        {
          Id = Convert.ToInt32(result.Rows[i]["Id"]),
          Description = result.Rows[i]["Description"].ToString(),
          ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
        };
        forbiddenWords.Add(forbiddenWord);
      }
      return forbiddenWords;
    }

    public List<string> GetWords()
    {
      var words = new List<string>();
      string word;
      var query = $"SELECT DESCRIPTION FROM FORBIDDENWORD";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        word = result.Rows[i]["Description"].ToString();
        words.Add(word);
      }
      return words;
    }
  }
}
