using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface ILogRepository
  {
    public List<LogGridViewModel> GetLogsGrid();
    public void Add(NewLogViewModel log);
  }
}
