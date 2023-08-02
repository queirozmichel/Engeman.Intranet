using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class UserAccountRepository : IUserAccountRepository
  {
    public UserAccount GetById(int id)
    {
      var query = $"SELECT * FROM USERACCOUNT WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      var userAccount = new UserAccount
      {
        Id = Convert.ToInt32(result.Rows[0]["id"]),
        Active = Convert.ToBoolean(result.Rows[0]["active"]),
        Name = result.Rows[0]["name"].ToString(),
        Username = result.Rows[0]["username"].ToString(),
        DepartmentId = Convert.ToInt32(result.Rows[0]["department_id"]),
        Email = result.Rows[0]["email"].ToString(),
        Photo = (byte[])result.Rows[0]["photo"],
        Description = result.Rows[0]["description"].ToString(),
        Permissions = result.Rows[0]["permissions"].ToString(),
        ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString())
      };

      return userAccount;
    }

    public UserAccount GetByUsername(string username)
    {
      var query = $"SELECT UA.ID,UA.ACTIVE,NAME,USERNAME,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
                  $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.PERMISSIONS, UA.CHANGE_DATE FROM USERACCOUNT UA INNER JOIN DEPARTMENT D " +
                  $"ON UA.DEPARTMENT_ID = D.ID WHERE UA.USERNAME = '{username.ToLower()}'";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      var userAccount = new UserAccount
      {
        Id = Convert.ToInt32(result.Rows[0]["id"]),
        Active = Convert.ToBoolean(result.Rows[0]["active"]),
        Name = result.Rows[0]["name"].ToString(),
        Username = result.Rows[0]["username"].ToString(),
        DepartmentId = Convert.ToInt32(result.Rows[0]["department_id"]),
        Email = result.Rows[0]["email"].ToString(),
        Photo = (byte[])result.Rows[0]["photo"],
        Description = result.Rows[0]["userdescription"].ToString(),
        Permissions = result.Rows[0]["permissions"].ToString(),
        ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString())
      };
      return userAccount;
    }

    public string GetUsernameById(int id)
    {
      var query = $"SELECT USERNAME FROM USERACCOUNT WHERE ID = {id} AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataToString(query);

      return result;
    }

    public List<UserGridViewModel> GetUsersGrid()
    {
      var users = new List<UserGridViewModel>();
      var query = $"SELECT U.ID, U.ACTIVE, U.NAME, U.USERNAME, D.DESCRIPTION AS DEPARTMENT FROM USERACCOUNT AS U INNER JOIN DEPARTMENT AS D " +
                  $"ON U.DEPARTMENT_ID = D.ID";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var user = new UserGridViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Active = Convert.ToBoolean(result.Rows[i]["active"]),
          Name = result.Rows[i]["name"].ToString(),
          Username = result.Rows[i]["username"].ToString(),
          Department = result.Rows[i]["department"].ToString(),
        };
        user.Moderator = GlobalFunctions.IsModerator(user.Id);
        users.Add(user);
      }

      return users;
    }

    public int GetDepartmentIdById(int id)
    {
      var query = $"SELECT DEPARTMENT_ID FROM USERACCOUNT WHERE ID = {id} AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataToInt(query);

      return result;
    }

    public void Add(NewUserViewModel user, string currentUsername)
    {
      var query = $"INSERT INTO USERACCOUNT (NAME, USERNAME, EMAIL, DEPARTMENT_ID, MODERATOR, NOVICE_USER) OUTPUT INSERTED.ID VALUES ('{user.Name}', '{user.Username}', " +
                  $"'{user.Email}', {user.DepartmentId}, {user.Moderator}, {user.NoviceUser})";

      using StaticQuery sq = new();
      var outputUserId = sq.GetDataToInt(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('I', "USU", outputUserId, "USERACCOUNT", currentUsername);
      }
    }

    public void Update(UserAccount userAccount)
    {
      string[] paramters = { "Photo;byte" };
      object[] values = { userAccount.Photo };
      var query = $"UPDATE USERACCOUNT SET NAME = '{userAccount.Name}', EMAIL = '{userAccount.Email}', DESCRIPTION = '{userAccount.Description}', " +
                  $"PHOTO = CONVERT(VARBINARY(MAX),@Photo) WHERE USERNAME = '{userAccount.Username}'";

      using StaticQuery sq = new StaticQuery();
      sq.ExecuteCommand(query, paramters, values);
    }

    public void UpdateByModerator(int id, UserAccount editedUser, string currentUsername)
    {
      string[] paramters = { "Photo;byte" };
      object[] values = { editedUser.Photo };
      var query = $"UPDATE USERACCOUNT SET ACTIVE = '{editedUser.Active}', NAME = '{editedUser.Name}', USERNAME = '{editedUser.Username}', EMAIL = '{editedUser.Email}', " +
                  $"DEPARTMENT_ID = {editedUser.DepartmentId}, DESCRIPTION = '{editedUser.Description}' , PERMISSIONS = '{editedUser.Permissions}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " + $"WHERE ID = {id}";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query, paramters, values);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('U', "USU", id, "USERACCOUNT", currentUsername);
      }
    }

    public void Delete(int id, string currentUsername)
    {
      string query = $"DELETE FROM USERACCOUNT WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('D', "USU", id, "USERACCOUNT", currentUsername);
      }
    }

    public string GetPermissionsById(int id)
    {
      var query = $"SELECT PERMISSIONS FROM USERACCOUNT WHERE ID = {id}";

      using StaticQuery sq = new();
      return sq.GetDataToString(query);
    }
  }
}