using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostCommentRepository
  {
    public void AddPostComment(NewCommentViewModel newComment);
    public List<Comment> GetPostCommentsByPostId(int postId);
    public List<Comment> GetPostCommentsByUserId(int userId);
    public Comment GetPostCommentById(int commentId);
    public List<Comment> GetUnrevisedComments();
    public List<Comment> GetPostCommentsByRestriction(UserAccount user, int postId);
    public bool DeletePostCommentById(int id);
    public bool UpdatePostCommentById(int id, Comment comment);
  }
}