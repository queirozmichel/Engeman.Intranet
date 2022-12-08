using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class DepartmentRepository : IDepartmentRepository
  {
    public Department GetById(int idDepartment)
    {
      Department department = new Department();
      var query = $"SELECT * FROM DEPARTMENT WHERE ID = {idDepartment}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        department.Id = Convert.ToInt32(result.Rows[0]["Id"]);
        department.Code = result.Rows[0]["Code"].ToString();
        department.Description = result.Rows[0]["Description"].ToString();
        department.Active = Convert.ToBoolean(result.Rows[0]["Active"]);
        department.ChangeDate = (DateTime)result.Rows[0]["Change_Date"];

        return department;
      }
    }

    public List<Department> Get()
    {
      List<Department> departments = new List<Department>();
      var query = $"SELECT * FROM DEPARTMENT";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          Department department = new Department();

          department.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          department.Code = result.Rows[i]["Code"].ToString();
          department.Description = result.Rows[i]["Description"].ToString();
          department.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
          department.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];

          departments.Add(department);
        }
        return departments;
      }
    }

    public string GetDescriptionById(int id)
    {
      var query = $"SELECT DESCRIPTION FROM DEPARTMENT WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataToString(query);

        return result;
      }
    }    
  }
}