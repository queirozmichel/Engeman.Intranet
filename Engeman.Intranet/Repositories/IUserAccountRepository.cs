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
    public string GetPermissionsById(int id);
    public int GetDepartmentIdById(int id);
    public void Add(NewUserViewModel user, string currentUsername = null);
    public void Update(UserAccount userAccount);
    public void UpdateByModerator(int id, UserAccount user, string currentUsername = null);
    public void Delete(int id, string currentUsername = null);
  }
}