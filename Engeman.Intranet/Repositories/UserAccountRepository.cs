using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Engeman.Intranet.Repositories
{
  public class UserAccountRepository : IUserAccountRepository
  {
    public UserAccountRepository()
    {

    }
    public bool UserAccountValidate(string domainUsername)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query = ("SELECT 0 FROM USERACCOUNT WHERE DOMAINACCOUNT = " + "'" + domainUsername + "' AND ACTIVE = 'S'").ToUpper();

        string result = sq.GetDataToString(query);

        if (result == "")
        {
          return false;
        } else
        {
          return true;
        }
      }
    }

    public UserProfile GetUserProfile(string domainUsername)
    {

      using (StaticQuery sq = new StaticQuery())
      {
        var query = ("SELECT NAME,DOMAINACCOUNT,D.DESCRIPTION AS DEPARTMENTDESCRIPTION,EMAIL,PHOTO,UA.DESCRIPTION AS USERDESCRIPTION " +
          "FROM ENGEMANINTRANET.USERACCOUNT UA INNER JOIN ENGEMANINTRANET.DEPARTMENT D " +
          "ON UA.DEPARTMENT_ID = D.ID where DOMAINACCOUNT = " + "'" + domainUsername + "'");

        var result = sq.GetDataSet(query);

        UserProfile userProfile = new UserProfile();

        userProfile.Name = result.Tables["TABLE"].Rows[0]["NAME"].ToString();
        userProfile.DomainAccount = result.Tables["TABLE"].Rows[0]["DOMAINACCOUNT"].ToString();
        userProfile.Department.Description = result.Tables["TABLE"].Rows[0]["DEPARTMENTDESCRIPTION"].ToString();
        userProfile.Email = result.Tables["TABLE"].Rows[0]["EMAIL"].ToString();
        userProfile.Photo = result.Tables["TABLE"].Rows[0]["PHOTO"].ToString();
        userProfile.Description = result.Tables["TABLE"].Rows[0]["USERDESCRIPTION"].ToString();

        return userProfile;
      }
    }
  }
}
