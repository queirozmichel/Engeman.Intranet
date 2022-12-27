using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface ILogRepository
  {
    public void Add(NewLogViewModel newLog);
    public List<string> GetFormatted();
  }
}
