using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface ICommentFileRepository
  {
    public List<CommentFile> GetByCommentId(int id);

    public void Add(int id, List<CommentFile> files);

    public void Delete(int id);
  }
}