using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public List<UserAccount> Get();
    public void Update(UserAccount userAccount);
    public UserAccount GetById(int id);
    public UserAccount GetByUsername(string username);
    public string GetUsernameById(int id);
    public UserPermissionsViewModel GetUserPermissionsByUsername(string username);
  }
}