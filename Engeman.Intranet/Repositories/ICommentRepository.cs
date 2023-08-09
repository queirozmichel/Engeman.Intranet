using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface ICommentRepository
  {
    public List<Comment> Get();
    public Comment GetById(int id);
    public List<Comment> GetByPostId(int postId);
    public List<Comment> GetByUserAccountId(int userAccountId);
    public List<Comment> GetByUsername(string username);
    public List<Comment> GetUnrevisedComments();
    public List<Comment> GetByUserRestriction(UserAccount user, int postId);
    public int GetPostIdById(int id);
    public int Add(NewCommentViewModel comment, string currentUsername = null);
    public void Update(int id, Comment comment, string currentUsername = null);
    public void Delete(int id, string currentUsername = null);
    public int Count();
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public void Aprove(int id, string currentUsername = null);
  }
}