using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IKeywordRepository
  {
    public List<KeywordViewModel> GetKeywordsGrid();
    public List<Keyword> Get();
    public List<KeywordComponentViewModel> GetIdAndName();
    public Keyword GetById(int id);
    public void Add(KeywordViewModel keyword, string currentUsername = null);
    public void Update(int id, Keyword keyword, string currentUsername = null);
    public void Delete(int id, string currentUsername = null);
  }
}
