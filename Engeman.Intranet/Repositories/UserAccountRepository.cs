using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System.Threading.Tasks;

namespace Engeman.Intranet.Repositories
{
  public class UserAccountRepository : IUserAccountRepository
  {
    public bool UserAccountValidate(Credentials credentials)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string result = sq.GetDataToString("SELECT 0 FROM USERACCOUNT WHERE DOMAINACCOUNT = " + "'" + credentials.DomainUsername + "'");

        if (result == "")
        {
          return false;
        } else
        {
          return true;
        }
      }
    }
  }
}
