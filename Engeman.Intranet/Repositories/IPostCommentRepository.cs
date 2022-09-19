using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentRepository
  {
    public void AddPostComment(PostComment postComment);
    public void AddPostComment(PostComment postComment, List<Archive> archives);
    public List<PostComment> GetPostCommentsByPostId(int postId);
    public bool DeletePostCommentById(int id);
  }
}