using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface ICommentFileRepository
  {
    public List<CommentFile> GetByCommentId(int commentId);
    public void Add(int commentId, List<CommentFile> files);
    public void Delete(int[] ids);
  }
}