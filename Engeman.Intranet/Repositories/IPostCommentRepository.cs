using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentRepository
  {
    void AddPostComment(PostComment postComment);
    void AddPostComment(PostComment postComment, List<Archive> archives);
  }
}