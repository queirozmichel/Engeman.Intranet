﻿using Engeman.Intranet.Library;
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
        var query = $"SELECT 0 FROM USER_ACCOUNT WHERE DOMAIN_ACCOUNT = '{domainUsername.ToUpper()}' AND ACTIVE = 1";
        string result = sq.GetDataToString(query);

        if (result == "")
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    public UserAccount GetUserAccountByDomainUsername(string domainUsername)
    {
      UserAccount userAccount = new UserAccount();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"UA.ID,UA.ACTIVE,NAME,DOMAIN_ACCOUNT,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
          $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CREATE_POST, UA.EDIT_OWNER_POST, UA.DELETE_OWNER_POST, UA.EDIT_ANY_POST, " +
          $"UA.DELETE_ANY_POST, UA.MODERATOR, UA.NOVICE_USER, UA.CHANGE_DATE " +
          $"FROM USER_ACCOUNT UA INNER JOIN DEPARTMENT D " +
          $"ON UA.DEPARTMENT_ID = D.ID " +
          $"WHERE DOMAIN_ACCOUNT = '{domainUsername.ToUpper()}'";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        userAccount.Id = Convert.ToInt32(result["id"]);
        userAccount.Active = Convert.ToBoolean(result["active"]);
        userAccount.Name = result["name"].ToString();
        userAccount.DomainAccount = result["domain_account"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result["department_id"]);
        userAccount.Email = result["email"].ToString();
        userAccount.Photo = (byte[])result["photo"];
        userAccount.Description = result["userdescription"].ToString();
        userAccount.CreatePost = Convert.ToBoolean(result["create_post"]);
        userAccount.EditOwnerPost = Convert.ToBoolean(result["edit_owner_post"]);
        userAccount.DeleteOwnerPost = Convert.ToBoolean(result["delete_owner_post"]);
        userAccount.EditAnyPost = Convert.ToBoolean(result["edit_any_post"]);
        userAccount.DeleteAnyPost = Convert.ToBoolean(result["delete_any_post"]);
        userAccount.Moderator = Convert.ToBoolean(result["moderator"]);
        userAccount.NoviceUser = Convert.ToBoolean(result["novice_user"]);
        userAccount.ChangeDate = Convert.ToDateTime(result["change_date"].ToString());

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
        $"UPDATE USER_ACCOUNT SET NAME = '{userAccount.Name}', EMAIL = '{userAccount.Email}'," +
        $"DESCRIPTION = '{userAccount.Description}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
        $"WHERE DOMAIN_ACCOUNT = '{userAccount.DomainAccount}'";

        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public List<UserAccountDto> GetAllUserAccounts()
    {
      List<UserAccountDto> users = new List<UserAccountDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT * FROM USER_ACCOUNT WHERE ACTIVE = 'S'";
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          UserAccountDto userAccountDto = new UserAccountDto();
          userAccountDto.Id = Convert.ToInt32(result.Rows[i]["id"]);
          userAccountDto.Name = result.Rows[i]["name"].ToString();
          userAccountDto.Email = result.Rows[i]["email"].ToString();
          userAccountDto.DomainAccount = result.Rows[i]["domain_account"].ToString();
          userAccountDto.ChangeDate = Convert.ToString(result.Rows[i]["change_date"]);

          users.Add(userAccountDto);
        }
      }
      return users;
    }

    public UserAccount GetUserAccountById(int id)
    {
      UserAccount userAccount = new UserAccount();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"* " +
          $"FROM USER_ACCOUNT " +
          $"WHERE ID = {id}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        userAccount.Id = Convert.ToInt32(result["id"]);
        userAccount.Active = Convert.ToBoolean(result["active"]);
        userAccount.Name = result["name"].ToString();
        userAccount.DomainAccount = result["domain_account"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result["department_id"]);
        userAccount.Email = result["email"].ToString();
        userAccount.Photo = (byte[])result["photo"];
        userAccount.Description = result["description"].ToString();
        userAccount.Moderator = Convert.ToBoolean(result["Moderator"]);
        userAccount.ChangeDate = Convert.ToDateTime(result["change_date"].ToString());

        return userAccount;
      }
    }

    public string GetUserAccountNameById(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT NAME " +
        $"FROM USER_ACCOUNT " +
        $"WHERE ID = {id} ";

        var result = sq.GetDataToString(query);
        return result;
      };
    }

    public UserPermissionsViewModel GetUserPermissionsByDomainUsername(string domainUsername)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        UserPermissionsViewModel userPermissions = new UserPermissionsViewModel();
        var query =
        $"SELECT CREATE_POST, EDIT_OWNER_POST, DELETE_OWNER_POST, EDIT_ANY_POST, DELETE_ANY_POST, MODERATOR, NOVICE_USER " +
        $"FROM USER_ACCOUNT " +
        $"WHERE DOMAIN_ACCOUNT = '{domainUsername}'";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        userPermissions.CreatePost = Convert.ToBoolean(result["create_post"]);
        userPermissions.EditOwnerPost = Convert.ToBoolean(result["edit_owner_post"]);
        userPermissions.DeleteOwnerPost = Convert.ToBoolean(result["delete_owner_post"]);
        userPermissions.EditAnyPost = Convert.ToBoolean(result["edit_any_post"]);
        userPermissions.DeleteAnyPost = Convert.ToBoolean(result["delete_any_post"]);
        userPermissions.Moderator = Convert.ToBoolean(result["moderator"]);
        userPermissions.NoviceUser = Convert.ToBoolean(result["novice_user"]);

        return userPermissions;
      }
    }

    public string GetDomainAccountById(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT DOMAIN_ACCOUNT " +
        $"FROM USER_ACCOUNT " +
        $"WHERE ID = {id} ";

        var result = sq.GetDataToString(query);
        return result;
      };
    }
  }
}
