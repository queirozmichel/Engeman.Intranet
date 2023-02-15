using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IForbiddenWordRepository
  {
    public List<ForbiddenWord> Get();
    public List<string> GetWords();
  }
}
