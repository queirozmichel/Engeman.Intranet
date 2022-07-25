using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IArchiveRepository
  {
    public List<Archive> GetArchiveByPostId(int postId);
    public void DeleteArchiveById(int id);
  }
}