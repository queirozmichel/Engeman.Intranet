using Engeman.Intranet.Library;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public class DepartmentRepository : IDepartmentRepository
  {
    public Department GetById(int id)
    {
      var query = $"SELECT * FROM DEPARTMENT WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      var department = new Department
      {
        Id = Convert.ToInt32(result.Rows[0]["Id"]),
        Code = result.Rows[0]["Code"].ToString(),
        Description = result.Rows[0]["Description"].ToString(),
        Active = Convert.ToBoolean(result.Rows[0]["Active"]),
        ChangeDate = (DateTime)result.Rows[0]["Change_Date"]
      };

      return department;
    }

    public List<Department> Get()
    {
      var departments = new List<Department>();
      var query = $"SELECT * FROM DEPARTMENT";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        Department department = new Department
        {
          Id = Convert.ToInt32(result.Rows[i]["Id"]),
          Code = result.Rows[i]["Code"].ToString(),
          Description = result.Rows[i]["Description"].ToString(),
          Active = Convert.ToBoolean(result.Rows[i]["Active"]),
          ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
        };
        departments.Add(department);
      }

      return departments;
    }

    public string GetDescriptionById(int id)
    {
      var query = $"SELECT DESCRIPTION FROM DEPARTMENT WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataToString(query);

      return result;
    }    
  }
}