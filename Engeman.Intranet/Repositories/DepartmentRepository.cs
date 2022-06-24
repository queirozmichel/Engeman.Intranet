using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;

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
          $"FROM ENGEMANINTRANET.DEPARTMENT " +
          $"WHERE ID = {idDepartment}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        department.Id = Convert.ToInt32(result["Id"]);
        department.Code = result["Code"].ToString();
        department.Description = result["Description"].ToString();
        department.Active = Convert.ToChar(result["Active"]);
        department.ChangeDate = (DateTime)result["ChangeDate"];

        return department;
      }
    }
  }
}
