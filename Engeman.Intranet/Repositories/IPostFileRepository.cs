using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostFileRepository
  {
    public List<PostFile> GetByPostId(int postId);
    public void Add(int postId, List<NewPostFileViewModel> files);
    public void Delete(int id);
  }
}