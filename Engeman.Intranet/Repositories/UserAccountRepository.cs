using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
  {
    public List<UserAccount> Get()
    {
      List<UserAccount> users = new List<UserAccount>();
      var query = "SELECT * FROM USERACCOUNT ";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          UserAccount user = new UserAccount();
          user.Id = Convert.ToInt32(result.Rows[i]["id"]);
          user.Active = Convert.ToBoolean(result.Rows[i]["active"]);
          user.Name = result.Rows[i]["name"].ToString();
          user.DomainAccount = result.Rows[i]["domain_account"].ToString();
          user.Email = result.Rows[i]["email"].ToString();
          user.Photo = (byte[])result.Rows[i]["photo"];
          user.Description = result.Rows[i]["description"].ToString();
          user.CreatePost = Convert.ToBoolean(result.Rows[i]["create_post"]);
          user.EditOwnerPost = Convert.ToBoolean(result.Rows[i]["edit_owner_post"]);
          user.DeleteOwnerPost = Convert.ToBoolean(result.Rows[i]["delete_owner_post"]);
          user.EditAnyPost = Convert.ToBoolean(result.Rows[i]["edit_any_post"]);
          user.DeleteAnyPost = Convert.ToBoolean(result.Rows[i]["delete_any_post"]);
          user.Moderator = Convert.ToBoolean(result.Rows[i]["moderator"]);
          user.NoviceUser = Convert.ToBoolean(result.Rows[i]["novice_user"]);
          user.ChangeDate = Convert.ToDateTime(result.Rows[i]["change_date"].ToString());
          users.Add(user);
        }
      }
      return users;
    }

    public void Update(UserAccount userAccount)
    {
      string[] paramters = { "Photo;byte" };
      Object[] values = { userAccount.Photo };
      var query =
      $"UPDATE USERACCOUNT SET NAME = '{userAccount.Name}', EMAIL = '{userAccount.Email}'," +
      $"DESCRIPTION = '{userAccount.Description}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
      $"WHERE DOMAIN_ACCOUNT = '{userAccount.DomainAccount}'";

      using (StaticQuery sq = new StaticQuery())
      {
        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public UserAccount GetById(int id)
    {
      UserAccount userAccount = new UserAccount();
      var query = $"SELECT * FROM USERACCOUNT WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        userAccount.Id = Convert.ToInt32(result.Rows[0]["id"]);
        userAccount.Active = Convert.ToBoolean(result.Rows[0]["active"]);
        userAccount.Name = result.Rows[0]["name"].ToString();
        userAccount.DomainAccount = result.Rows[0]["domain_account"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result.Rows[0]["department_id"]);
        userAccount.Email = result.Rows[0]["email"].ToString();
        userAccount.Photo = (byte[])result.Rows[0]["photo"];
        userAccount.Description = result.Rows[0]["description"].ToString();
        userAccount.Moderator = Convert.ToBoolean(result.Rows[0]["Moderator"]);
        userAccount.ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString());

        return userAccount;
      }
    }

    public UserAccount GetByDomainUsername(string domainUsername)
    {
      UserAccount userAccount = new UserAccount();
      var query =
      $"SELECT " +
      $"UA.ID,UA.ACTIVE,NAME,DOMAIN_ACCOUNT,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
      $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CREATE_POST, UA.EDIT_OWNER_POST, UA.DELETE_OWNER_POST, UA.EDIT_ANY_POST, " +
      $"UA.DELETE_ANY_POST, UA.MODERATOR, UA.NOVICE_USER, UA.CHANGE_DATE " +
      $"FROM USERACCOUNT UA INNER JOIN DEPARTMENT D " +
      $"ON UA.DEPARTMENT_ID = D.ID " +
      $"WHERE DOMAIN_ACCOUNT = '{domainUsername.ToUpper()}'";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          userAccount = null; 
          return userAccount;
        }

        userAccount.Id = Convert.ToInt32(result.Rows[0]["id"]);
        userAccount.Active = Convert.ToBoolean(result.Rows[0]["active"]);
        userAccount.Name = result.Rows[0]["name"].ToString();
        userAccount.DomainAccount = result.Rows[0]["domain_account"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result.Rows[0]["department_id"]);
        userAccount.Email = result.Rows[0]["email"].ToString();
        userAccount.Photo = (byte[])result.Rows[0]["photo"];
        userAccount.Description = result.Rows[0]["userdescription"].ToString();
        userAccount.CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]);
        userAccount.EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]);
        userAccount.DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]);
        userAccount.EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]);
        userAccount.DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]);
        userAccount.Moderator = Convert.ToBoolean(result.Rows[0]["moderator"]);
        userAccount.NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"]);
        userAccount.ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString());

        return userAccount;
      }
    }

    public string GetDomainAccountById(int id)
    {
      var query = $"SELECT DOMAIN_ACCOUNT FROM USERACCOUNT WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataToString(query);

        return result;
      }
    }

    public UserPermissionsViewModel GetUserPermissionsByDomainUsername(string domainUsername)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        UserPermissionsViewModel userPermissions = new UserPermissionsViewModel();
        var query =
        $"SELECT CREATE_POST, EDIT_OWNER_POST, DELETE_OWNER_POST, EDIT_ANY_POST, DELETE_ANY_POST, MODERATOR, NOVICE_USER " +
        $"FROM USERACCOUNT " +
        $"WHERE DOMAIN_ACCOUNT = '{domainUsername}'";

        var result = sq.GetDataSet(query).Tables[0];

        userPermissions.CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]);
        userPermissions.EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]);
        userPermissions.DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]);
        userPermissions.EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]);
        userPermissions.DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]);
        userPermissions.Moderator = Convert.ToBoolean(result.Rows[0]["moderator"]);
        userPermissions.NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"]);

        return userPermissions;
      }
    }
  }
}