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
    public int Add(NewCommentViewModel comment);
    public void AddWithLog(NewCommentViewModel comment, string currentUsername);
    public void Update(int id, Comment comment);
    public void UpdateWithLog(int id, Comment comment, string currentUsername);
    public void Delete(int id);
    public void DeleteWithLog(int id, string currentUsername);
    public int Count();
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public void Aprove(int id);
    public void AproveWithLog(int id, string currentUsername);
  }
}