using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
    public interface IPostRepository
  {
    public List<PostGridViewModel> GetByRestriction(UserAccount user);
    public List<PostGridViewModel> GetWithUnrevisedComments();
    public List<Post> GetByUserAccountId(int userAccountId);
    public Post Get(int id);
    public void Update(PostEditViewModel editedPost);
    public void Add(NewPostViewModel newPost);
    public void Delete(int postId);
    public bool Update(int id, Post post);
  }
}