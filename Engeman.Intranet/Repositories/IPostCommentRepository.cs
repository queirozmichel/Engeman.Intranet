using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentRepository
  {
    public void AddPostComment(NewCommentViewModel newComment);
    public List<PostComment> GetPostCommentsByPostId(int postId);
    public PostComment GetPostCommentById(int commentId);
    public List<PostComment> GetUnrevisedComments();
    public List<PostComment> GetPostCommentsByRestriction(UserAccount user, int postId);
    public bool DeletePostCommentById(int id);
    public bool UpdatePostCommentById(int id, PostComment comment);
    public bool UpdatePostCommentById(int id, PostComment comment, List<CommentFile> files);
  }
}