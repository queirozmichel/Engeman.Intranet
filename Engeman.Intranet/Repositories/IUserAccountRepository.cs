using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
    public interface IUserAccountRepository
  {
    public List<UserAccount> Get();
    public void Update(UserAccount userAccount);
    public UserAccount GetById(int id);
    public UserAccount GetByDomainUsername(string domainUsername);
    public string GetDomainAccountById(int id);
    public UserPermissionsViewModel GetUserPermissionsByDomainUsername(string domainUsername);
  }
}