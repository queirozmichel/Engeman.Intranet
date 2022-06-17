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

    public UserAccount GetUserAccount(string domainUsername)
    {
      UserAccount userAccount = new UserAccount();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"UA.ID,UA.ACTIVE,NAME,DOMAINACCOUNT,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
          $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CHANGEDATE " +
          $"FROM ENGEMANINTRANET.USERACCOUNT UA INNER JOIN ENGEMANINTRANET.DEPARTMENT D " +
          $"ON UA.DEPARTMENT_ID = D.ID " +
          $"WHERE DOMAINACCOUNT = '{domainUsername.ToUpper()}'";

        var result =  sq.GetDataSet(query).Tables[0].Rows[0];

        userAccount.Id = Convert.ToInt32(result["ID"]);
        userAccount.Active = Convert.ToChar(result["active"]);
        userAccount.Name = result["name"].ToString();
        userAccount.DomainAccount = result["domainaccount"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result["department_id"]);
        userAccount.Department.Description = result["department_description"].ToString();
        userAccount.Email = result["email"].ToString();
        userAccount.Photo = (byte[])result["photo"];
        userAccount.Description = result["userdescription"].ToString();
        userAccount.ChangeDate = Convert.ToDateTime(result["changedate"].ToString());

        return userAccount;
      }
    }
    public void UpdateUserAccount(UserAccount userAccount)
    {

      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "Photo;byte" };
        Object[] values = { userAccount.Photo };

        var query =
        $"UPDATE ENGEMANINTRANET.USERACCOUNT SET NAME = '{userAccount.Name}', EMAIL = '{userAccount.Email}'," +
        $"DESCRIPTION = '{userAccount.Description}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
        $"WHERE DOMAINACCOUNT = '{userAccount.DomainAccount}'";

        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public List<UserAccountDto> GetAllUserAccounts()
    {
      List<UserAccountDto> users = new List<UserAccountDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT * FROM ENGEMANINTRANET.USERACCOUNT WHERE ACTIVE = 'S'";
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          UserAccountDto userAccountDto = new UserAccountDto();
          userAccountDto.Id = Convert.ToInt32(result.Rows[i]["id"]);
          userAccountDto.Name = result.Rows[i]["name"].ToString();
          userAccountDto.Email = result.Rows[i]["email"].ToString();
          userAccountDto.DomainAccount = result.Rows[i]["domainaccount"].ToString();
          userAccountDto.ChangeDate = Convert.ToString(result.Rows[i]["changeDate"]);

          users.Add(userAccountDto);
        }
      }
     return users;
    }    
  }
}
