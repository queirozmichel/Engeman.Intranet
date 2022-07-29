using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IDepartmentRepository
  {
    public Department GetDepartmentById(int id);
    public List<Department> GetAllDepartments();
  }
}
