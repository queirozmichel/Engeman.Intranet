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
          Operation = result.Rows[i]["operation"].ToString(),
          RegistryType = result.Rows[i]["registry_type"].ToString(),
          Username = result.Rows[i]["username"].ToString(),
          ChangeDate = result.Rows[i]["change_date"].ToString()
      };
        if (result.Rows[i]["registry_id"] is DBNull) log.RegistryId = null;
        else log.RegistryId = Convert.ToInt32(result.Rows[i]["registry_id"]);
        log.RegistryTable = result.Rows[i]["registry_table"].ToString();
        logs.Add(log);
      }
      return logs;
    }
  }
}
