using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public bool UserAccountValidate(string domainUsername);
    public UserProfile GetUserProfile(string domainUsername);
    public void UpdateUserProfile(UserProfile userProfile);
  }
}