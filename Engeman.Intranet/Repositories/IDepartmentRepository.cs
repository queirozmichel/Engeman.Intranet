using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IDepartmentRepository
  {
    public Department GetDepartmentById(int id);
  }
}
