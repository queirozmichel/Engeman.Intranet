using Engeman.Intranet.Library;

namespace Engeman.Intranet.Helpers
{
  public class GlobalFunctions
  {
    public static void NewLog(char operation, string registry_type, int registry_id, string registry_table, string username)
    {
      var query = $"EXEC NewLog '{operation}','{registry_type}',{registry_id},'{registry_table}','{username}'";
      using StaticQuery sq = new();

      sq.ExecuteCommand(query);
    }
  }
}