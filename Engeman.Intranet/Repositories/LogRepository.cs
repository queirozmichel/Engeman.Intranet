using Engeman.Intranet.Library;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class LogRepository : ILogRepository
  {
    public void Add(NewLogViewModel newLog)
    {
      string query;

      if (newLog.ReferenceId != null && newLog.ReferenceTable != null)
      {
        query = $"INSERT INTO LOG (USERNAME, OPERATION, DESCRIPTION, REFERENCE_ID, REFERENCE_TABLE) VALUES ('{newLog.Username}', '{newLog.Operation}', " +
                $"'{newLog.Description}', {newLog.ReferenceId} , '{newLog.ReferenceTable}')";
      }
      else
      {
        query = $"INSERT INTO LOG (USERNAME, OPERATION, DESCRIPTION) VALUES ('{newLog.Username}', '{newLog.Operation}', " +
                $"'{newLog.Description}')";
      }

      using (StaticQuery sq = new StaticQuery())
      {
        sq.ExecuteCommand(query);
      }
    }

    public List<LogGridViewModel> GetLogsGrid()
    {
      var query = "SELECT * FROM LOG";
      var logs = new List<LogGridViewModel>();
      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var log = new LogGridViewModel();

          log.Id = Convert.ToInt32(result.Rows[i]["id"]);
          log.Username = result.Rows[i]["username"].ToString();
          log.Operation = result.Rows[i]["operation"].ToString();
          log.Description = result.Rows[i]["description"].ToString();
          if (result.Rows[i]["reference_id"] is DBNull) log.ReferenceId = null;      
          else log.ReferenceId = Convert.ToInt32(result.Rows[i]["reference_id"]);    
          log.ReferenceTable = result.Rows[i]["reference_table"].ToString();
          log.ChangeDate = result.Rows[i]["change_date"].ToString();
          logs.Add(log);
        }
        return logs;
      }
    }
  }
}
