using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IArchiveRepository
  {
    public Archive GetArchiveById(int id);
  }
}