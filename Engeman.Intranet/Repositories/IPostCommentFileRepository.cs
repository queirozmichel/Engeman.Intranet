using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentFileRepository
  {
    public IEnumerable<PostCommentFile> GetFilesByPostCommentId(int id);
  }
}