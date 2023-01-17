using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface ICommentRepository
  {
    public Comment GetById(int id);
    public List<Comment> GetByPostId(int postId);
    public List<Comment> GetByUserAccountId(int userAccountId);
    public List<Comment> GetUnrevisedComments();
    public List<Comment> GetByRestriction(UserAccount user, int postId);
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public int Add(NewCommentViewModel comment);
    public void AddWithLog(NewCommentViewModel comment, string currentUsername);
    public bool Delete(int id);
    public bool DeleteWithLog(int id, string currentUsername);
    public bool Update(int id, Comment comment);
    public void UpdateWithLog(int id, Comment comment, string currentUsername);
    public void Aprove(int id);
    public void AproveWithLog(int id, string currentUsername);
  }
}