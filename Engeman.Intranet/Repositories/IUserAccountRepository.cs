using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public List<UserAccount> Get();
    public List<UserGridViewModel> GetUsersGrid();
    public void Update(UserAccount userAccount);
    public UserAccount GetById(int id);
    public UserAccount GetByUsername(string username);
    public string GetUsernameById(int id);
    public UserPermissionsViewModel GetUserPermissionsByUsername(string username);
    public int Add(NewUserViewModel newUser);
    public void AddWithLog(NewUserViewModel newUser, string currentUsername);
    public int Remove(int userId);
    public int RemoveWithLog(int id, string currentUsername);
    public int UpdateByModerator(int id, UserAccount editedUser);
    public void UpdateByModeratorWithLog(int id, UserAccount userAccount, string currentUsername);
  }
}