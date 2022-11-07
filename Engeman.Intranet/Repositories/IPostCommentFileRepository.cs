using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentFileRepository
  {
    public List<CommentFile> GetFilesByPostCommentId(int id);

    public void AddFilesToComment(int id, List<CommentFile> files);

    public void DeleteFileById(int id);
  }
}