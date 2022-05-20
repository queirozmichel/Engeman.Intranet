using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public interface IUserAccountRepository
  {
    bool UserAccountValidate(Credentials credentials);
  }
}