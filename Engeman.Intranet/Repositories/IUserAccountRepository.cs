using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    public bool UserAccountValidate(string domainUsername);
    public UserProfile GetUserProfile(string domainUsername);
    public void UpdateUserProfile(UserProfile userProfile);
    public List<UserProfileDto> GetAllUserProfiles();
  }
}