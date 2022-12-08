using Engeman.Intranet.Library;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostRestrictionRepository : IPostRestrictionRepository
  {
    public List<int> GetDepartmentsByIdPost(int id)
    {
      List<int> departments = new List<int>();
      var query = $"SELECT DEPARTMENT_ID FROM POSTRESTRICTION WHERE POST_ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          departments.Add(Convert.ToInt32(result.Rows[i]["Department_Id"]));
        }
        return departments;
      }
    }
  }
}
