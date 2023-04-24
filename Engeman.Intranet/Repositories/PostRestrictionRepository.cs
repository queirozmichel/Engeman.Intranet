using Engeman.Intranet.Library;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public class PostRestrictionRepository : IPostRestrictionRepository
  {
    public List<int> GetDepartmentsByIdPost(int postId)
    {
      var departments = new List<int>();
      var query = $"SELECT DEPARTMENT_ID FROM POSTRESTRICTION WHERE POST_ID = {postId}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        departments.Add(Convert.ToInt32(result.Rows[i]["Department_Id"]));
      }
      return departments;
    }
    public int CountByPostIdDepId(int postId, int departmentId)
    {
      var query = $"SELECT COUNT(*) FROM POSTRESTRICTION WHERE POST_ID = {postId} AND DEPARTMENT_ID = {departmentId}";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }
  }
}
