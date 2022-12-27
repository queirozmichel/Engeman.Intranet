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

    public List<string> GetFormatted()
    {
      var query = "SELECT * FROM LOG";
      List<string> logs = new List<string>();
      string log = null;
      string referenceId = null;
      string referenceTable = null;

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          referenceId = result.Rows[i]["Reference_Id"].ToString() != "" ? result.Rows[i]["Reference_Id"].ToString() : "NULL";
          referenceTable = result.Rows[i]["Reference_Table"].ToString() != "" ? result.Rows[i]["Reference_Table"].ToString() : "NULL";

          log = $"Data: {Convert.ToDateTime(result.Rows[i]["Change_Date"]).ToShortDateString()} {Convert.ToDateTime(result.Rows[i]["Change_Date"]).ToShortTimeString()}" +
                $" | Usuário: {result.Rows[i]["Username"]} | Descrição: {result.Rows[i]["Operation"]} {result.Rows[i]["Description"]}" +
                $" | Id de referência: {referenceId} | Tabela de referência: {referenceTable}";
          logs.Add(log);
        }
        return logs;
      }
    }
  }
}
