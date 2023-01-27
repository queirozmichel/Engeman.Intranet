using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IDepartmentRepository
  {
    public List<Department> Get();
    public Department GetById(int id);
    public string GetDescriptionById(int id);    
  }
}