using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public bool UserAccountValidate(string domainUsername);
    public UserAccount GetUserAccount(string domainUsername);
    public void UpdateUserAccount(UserAccount userAccount);
    public List<UserAccountDto> GetAllUserAccounts();
  }
}