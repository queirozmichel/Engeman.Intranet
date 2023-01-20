using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostGridViewModel> GetByRestriction(UserAccount user);
    public List<PostGridViewModel> GetWithUnrevisedComments();
    public List<Post> GetByUserAccountId(int userAccountId);
    public List<Post> GetByUsername(string username);
    public string GetSubjectById(int id);
    public Post Get(int id);
    public int CountByUsername(string username);
    public int CountByUserId(int userId);
    public int CountByPostType(char postType);
    public void Update(PostEditViewModel editedPost);
    public void UpdateWithLog(PostEditViewModel editedPost, string currentUsername);
    public int Add(NewPostViewModel newPost);
    public void AddWithLog(NewPostViewModel newPost, string currentUsername);
    public void Delete(int postId);
    public void DeleteWithLog(int postId, string currentUsername);
    public bool Update(int id, Post post);
    public void Aprove(int id);
    public void AproveWithLog(int id, string currentUsername);
  }
}