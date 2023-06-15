using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IPostKeywordRepository
  {
    public string[] GetKeywordsByPostId(int id);
    public List<KeywordComponentViewModel> GetIdAndNameByPostId(int id);
  }
}
