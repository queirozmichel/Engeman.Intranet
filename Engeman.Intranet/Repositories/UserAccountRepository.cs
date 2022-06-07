using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engeman.Intranet.Repositories
{
  public class UserAccountRepository : IUserAccountRepository
  {   
    public bool UserAccountValidate(string domainUsername)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query = $"SELECT 0 FROM USERACCOUNT WHERE DOMAINACCOUNT = '{domainUsername.ToUpper()}' AND ACTIVE = 'S'";
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
      UserProfile userProfile = new UserProfile();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"UA.ID,UA.ACTIVE,NAME,DOMAINACCOUNT,D.DESCRIPTION AS DEPARTMENTDESCRIPTION,EMAIL," +
          $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CHANGEDATE " +
          $"FROM ENGEMANINTRANET.USERACCOUNT UA INNER JOIN ENGEMANINTRANET.DEPARTMENT D " +
          $"ON UA.DEPARTMENT_ID = D.ID where DOMAINACCOUNT = '{domainUsername.ToUpper()}'";

        var result =  sq.GetDataSet(Convert.ToString(query)).Tables[0].Rows[0];

        userProfile.Id = Convert.ToInt32(result["ID"]);
        userProfile.Active = Convert.ToChar(result["active"]);
        userProfile.Name = result["name"].ToString();
        userProfile.DomainAccount = result["domainaccount"].ToString();
        userProfile.Department.Description = result["departmentdescription"].ToString();
        userProfile.Email = result["email"].ToString();
        userProfile.Photo = (byte[])result["photo"];
        userProfile.Description = result["userdescription"].ToString();
        userProfile.ChangeDate = Convert.ToDateTime(result["changedate"].ToString());

        return userProfile;
      }
    }
    public void UpdateUserProfile(UserProfile userProfile)
    {

      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "Photo;byte" };
        Object[] values = { userProfile.Photo };

        var query =
        $"UPDATE ENGEMANINTRANET.USERACCOUNT SET NAME = '{userProfile.Name}', EMAIL = '{userProfile.Email}'," +
        $"DESCRIPTION = '{userProfile.Description}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
        $"WHERE DOMAINACCOUNT = '{userProfile.DomainAccount}'";

        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public List<UserProfileDto> GetAllUserProfiles()
    {
      List<UserProfileDto> users = new List<UserProfileDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT * FROM ENGEMANINTRANET.USERACCOUNT WHERE ACTIVE = 'S'";
        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          UserProfileDto userProfile = new UserProfileDto();
          userProfile.Name = result.Rows[i]["name"].ToString();
          userProfile.Email = result.Rows[i]["email"].ToString();
          userProfile.DomainAccount = result.Rows[i]["domainaccount"].ToString();

          users.Add(userProfile);
        }
      }
      return users;
    }
  }
}
