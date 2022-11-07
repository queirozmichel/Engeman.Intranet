using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class DepartmentRepository : IDepartmentRepository
  {
    public Department GetDepartmentById(int idDepartment)
    {
      Department department = new Department();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"* " +
          $"FROM DEPARTMENT " +
          $"WHERE ID = {idDepartment}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        department.Id = Convert.ToInt32(result["Id"]);
        department.Code = result["Code"].ToString();
        department.Description = result["Description"].ToString();
        department.Active = Convert.ToBoolean(result["Active"]);
        department.ChangeDate = (DateTime)result["Change_Date"];

        return department;
      }
    }

    public List<Department> GetAllDepartments()
    {
      var query = "";
      using (StaticQuery sq = new StaticQuery())
      {
        List<Department> departments = new List<Department>();        
        query =
        $"SELECT * " +
        $"FROM DEPARTMENT ";

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

    public string GetDepartmentNameById(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT DESCRIPTION " +
        $"FROM DEPARTMENT " +
        $"WHERE ID = {id}";

        var result = sq.GetDataToString(query);
        return result;
      }
    }
  }
}
