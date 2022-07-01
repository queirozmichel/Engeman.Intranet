using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IArchiveRepository
  {
    public Archive GetArchiveByPostId(int postId);
  }
}