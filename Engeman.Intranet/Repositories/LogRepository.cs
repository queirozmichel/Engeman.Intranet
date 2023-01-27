using Engeman.Intranet.Library;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class LogRepository : ILogRepository
  {
    public List<LogGridViewModel> GetLogsGrid()
    {
      var logs = new List<LogGridViewModel>();
      var query = $"SELECT * FROM LOG";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var log = new LogGridViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Username = result.Rows[i]["username"].ToString(),
          Operation = result.Rows[i]["operation"].ToString(),
          Description = result.Rows[i]["description"].ToString(),
          ChangeDate = result.Rows[i]["change_date"].ToString()
      };
        if (result.Rows[i]["reference_id"] is DBNull) log.ReferenceId = null;
        else log.ReferenceId = Convert.ToInt32(result.Rows[i]["reference_id"]);
        log.ReferenceTable = result.Rows[i]["reference_table"].ToString();
        logs.Add(log);
      }
      return logs;
    }

    public void Add(NewLogViewModel log)
    {
      string query;

      if (log.ReferenceId != null && log.ReferenceTable != null)
      {
        query = $"INSERT INTO LOG (USERNAME, OPERATION, DESCRIPTION, REFERENCE_ID, REFERENCE_TABLE) VALUES ('{log.Username}', '{log.Operation}', " +
                $"'{log.Description}', {log.ReferenceId} , '{log.ReferenceTable}')";
      }
      else query = $"INSERT INTO LOG (USERNAME, OPERATION, DESCRIPTION) VALUES ('{log.Username}', '{log.Operation}', '{log.Description}')";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query);
    }
  }
}
