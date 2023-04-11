using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IBlacklistTermRepository
  {
    public List<BlacklistTerm> Get();
    public List<string> GetTerms();
  }
}
