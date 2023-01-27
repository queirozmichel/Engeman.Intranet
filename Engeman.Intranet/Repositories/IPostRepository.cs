using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<Post> Get();
    public List<PostGridViewModel> GetPostsGrid(UserAccount user);
    public List<PostGridViewModel> GetWithUnrevisedComments();
    public List<Post> GetByUserAccountId(int userAccountId);
    public List<Post> GetByUsername(string username);
    public string GetSubjectById(int id);
    public Post GetById(int id);
    public int Add(NewPostViewModel post);
    public void AddWithLog(NewPostViewModel post, string currentUsername);
    public void Update(PostEditViewModel post);
    public void UpdateWithLog(int id, PostEditViewModel post, string currentUsername);
    public void Delete(int id);
    public void DeleteWithLog(int id, string currentUsername);
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public int CountByPostType(char postType);
    public void Aprove(int id);
    public void AproveWithLog(int id, string currentUsername);
  }
}