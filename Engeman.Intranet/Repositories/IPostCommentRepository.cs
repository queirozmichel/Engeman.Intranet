using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentRepository
  {
    public void AddPostComment(PostComment postComment);
    public void AddPostComment(PostComment postComment, List<PostFile> files);
    public List<PostComment> GetPostCommentsByPostId(int postId);
    public PostComment GetPostCommentById(int postId);
    public List<PostComment> GetUnrevisedComments();
    public List<PostComment> GetPostCommentsByRestriction(UserAccount user, int postId);
    public bool DeletePostCommentById(int id);
    public bool UpdatePostCommentById(int id, PostComment comment);
  }
}