using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface ILogRepository
  {
    public List<LogGridViewModel> GetLogsGrid();
  }
}
