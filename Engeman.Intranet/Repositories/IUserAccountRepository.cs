using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public List<UserGridViewModel> GetUsersGrid();
    public UserAccount GetById(int id);
    public UserAccount GetByUsername(string username);
    public string GetUsernameById(int id);
    public UserPermissionsViewModel GetUserPermissionsByUsername(string username);
    public int Add(NewUserViewModel user);
    public void AddWithLog(NewUserViewModel user, string currentUsername);
    public void Update(UserAccount userAccount);
    public void UpdateByModerator(int id, UserAccount user);
    public void UpdateByModeratorWithLog(int id, UserAccount user, string currentUsername);
    public void Delete(int id);
    public void DeleteWithLog(int id, string currentUsername);
  }
}