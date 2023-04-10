using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<Post> Get();
    public List<PostGridViewModel> GetPostsGrid(UserAccount user, string searchPhrase = null);
    public List<PostGridViewModel> GetWithUnrevisedComments();
    public List<Post> GetByUserAccountId(int userAccountId);
    public List<Post> GetByUsername(string username);
    public string GetSubjectById(int id);
    public Post GetById(int id);
    public void Add(NewPostViewModel post, string currentUsername = null);
    public void Update(int id, PostEditViewModel post, string currentUsername = null);
    public void Delete(int id, string currentUsername = null);
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public int CountByPostType(char postType);
    public void Aprove(int id, string currentUsername = null);
  }
}