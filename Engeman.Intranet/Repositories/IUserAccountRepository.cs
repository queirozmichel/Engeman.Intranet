using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public bool UserAccountValidate(string domainUsername);
    public UserAccount GetUserAccountByDomainUsername(string domainUsername);
    public UserAccount GetUserAccountById(int id);
    public string GetUserAccountNameById(int id);
    public void UpdateUserAccount(UserAccount userAccount);
    public List<UserAccountDto> GetAllUserAccounts();
  }
}