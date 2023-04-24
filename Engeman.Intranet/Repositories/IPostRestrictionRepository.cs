using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRestrictionRepository
  {
    public List<int> GetDepartmentsByIdPost(int idPost);
    public int CountByPostIdDepId(int postId, int departmentId);
  }
}