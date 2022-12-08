using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface ICommentRepository
  {
    public Comment GetById(int id);
    public List<Comment> GetByPostId(int postId);
    public List<Comment> GetByUserAccountId(int userAccountId);
    public List<Comment> GetUnrevisedComments();
    public List<Comment> GetByRestriction(UserAccount user, int postId);
    public void Add(NewCommentViewModel comment);
    public bool Delete(int id);
    public bool Update(int id, Comment comment);
  }
}