using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostFileRepository
  {
    public List<PostFile> GetFilesByPostId(int postId);
    public void DeleteFileById(int id);
  }
}