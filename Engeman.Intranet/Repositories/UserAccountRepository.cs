using Engeman.Intranet.Extensions;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class UserAccountRepository : IUserAccountRepository
  {
    private readonly ILogRepository _logRepository;

    public UserAccountRepository(ILogRepository logRepository)
    {
      _logRepository = logRepository;
    }

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
          user.Username = result.Rows[i]["username"].ToString();
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
      $"WHERE USERNAME = '{userAccount.Username}'";

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
        userAccount.Username = result.Rows[0]["username"].ToString();
        userAccount.DepartmentId = Convert.ToInt32(result.Rows[0]["department_id"]);
        userAccount.Email = result.Rows[0]["email"].ToString();
        userAccount.Photo = (byte[])result.Rows[0]["photo"];
        userAccount.Description = result.Rows[0]["description"].ToString();
        userAccount.Moderator = Convert.ToBoolean(result.Rows[0]["Moderator"]);
        userAccount.NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"]);
        userAccount.CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]);
        userAccount.EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]);
        userAccount.DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]);
        userAccount.EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]);
        userAccount.DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]);
        userAccount.ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString());

        return userAccount;
      }
    }

    public UserAccount GetByUsername(string username)
    {
      UserAccount userAccount = new UserAccount();
      var query =
      $"SELECT " +
      $"UA.ID,UA.ACTIVE,NAME,USERNAME,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
      $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CREATE_POST, UA.EDIT_OWNER_POST, UA.DELETE_OWNER_POST, UA.EDIT_ANY_POST, " +
      $"UA.DELETE_ANY_POST, UA.MODERATOR, UA.NOVICE_USER, UA.CHANGE_DATE " +
      $"FROM USERACCOUNT UA INNER JOIN DEPARTMENT D " +
      $"ON UA.DEPARTMENT_ID = D.ID " +
      $"WHERE USERNAME = '{username.ToUpper()}'";

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
        userAccount.Username = result.Rows[0]["username"].ToString();
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

    public string GetUsernameById(int id)
    {
      var query = $"SELECT USERNAME FROM USERACCOUNT WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataToString(query);

        return result;
      }
    }

    public UserPermissionsViewModel GetUserPermissionsByUsername(string username)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        UserPermissionsViewModel userPermissions = new UserPermissionsViewModel();
        var query =
        $"SELECT CREATE_POST, EDIT_OWNER_POST, DELETE_OWNER_POST, EDIT_ANY_POST, DELETE_ANY_POST, MODERATOR, NOVICE_USER " +
        $"FROM USERACCOUNT " +
        $"WHERE USERNAME = '{username}'";

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

    public List<UserGridViewModel> GetUsersGrid()
    {
      var query = "SELECT U.ID, U.ACTIVE, U.NAME, U.USERNAME, D.DESCRIPTION AS DEPARTMENT, U.MODERATOR, U.NOVICE_USER " +
        "FROM USERACCOUNT AS U INNER JOIN DEPARTMENT AS D " +
        "ON U.DEPARTMENT_ID = D.ID";
      var users = new List<UserGridViewModel>();

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          var user = new UserGridViewModel();

          user.Id = Convert.ToInt32(result.Rows[i]["id"]);
          user.Active = Convert.ToBoolean(result.Rows[i]["active"]);
          user.Name = result.Rows[i]["name"].ToString();
          user.Username = result.Rows[i]["username"].ToString();
          user.Department = result.Rows[i]["department"].ToString();
          user.Novice = Convert.ToBoolean(result.Rows[i]["novice_user"]);
          user.Moderator = Convert.ToBoolean(result.Rows[i]["moderator"]);
          users.Add(user);
        }
        return users;
      }
    }

    public int Add(NewUserViewModel newUser)
    {
      var query = $"INSERT INTO USERACCOUNT (NAME, USERNAME, EMAIL, DEPARTMENT_ID, MODERATOR, NOVICE_USER) OUTPUT INSERTED.ID VALUES ('{newUser.Name}', '{newUser.Username}', " +
                  $"'{newUser.Email}', {newUser.DepartmentId}, {newUser.Moderator}, {newUser.NoviceUser})";
      using (StaticQuery sq = new StaticQuery())
      {
        var outputUserId = sq.GetDataToInt(query);
        return outputUserId;
      }
    }

    public void AddWithLog(NewUserViewModel newUser, string currentUsername)
    {
      var outputUserId = Add(newUser);
      var newLog = new NewLogViewModel(currentUsername, Operation.Inclusion.GetEnumDescription(), outputUserId, ReferenceTable.UserAccount.GetEnumDescription());
      newLog.Description = "de usuário";
      _logRepository.Add(newLog);
    }

    public int Remove(int userId)
    {
      string query = $"DELETE FROM USERACCOUNT WHERE ID = {userId}";
      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.ExecuteCommand(query);
        return result;
      }
    }

    public int RemoveWithLog(int id, string currentUsername)
    {
      int result = Remove(id);
      var newLog = new NewLogViewModel(currentUsername, Operation.Exclusion.GetEnumDescription(), id, ReferenceTable.UserAccount.GetEnumDescription());
      newLog.Description = "de usuário";
      _logRepository.Add(newLog);
      return result;
    }

    public int UpdateByModerator(int id, UserAccount editedUser)
    {
      string[] paramters = { "Photo;byte" };
      Object[] values = { editedUser.Photo };
      var query = $"UPDATE USERACCOUNT SET ACTIVE = '{editedUser.Active}', NAME = '{editedUser.Name}', USERNAME = '{editedUser.Username}', EMAIL = '{editedUser.Email}', " +
                  $"DEPARTMENT_ID = {editedUser.DepartmentId}, DESCRIPTION = '{editedUser.Description}', CREATE_POST = '{editedUser.CreatePost}', " +
                  $"EDIT_OWNER_POST = '{editedUser.EditOwnerPost}', DELETE_OWNER_POST = '{editedUser.DeleteOwnerPost}', EDIT_ANY_POST = '{editedUser.EditAnyPost}', " +
                  $"DELETE_ANY_POST = '{editedUser.DeleteAnyPost}', MODERATOR = '{editedUser.Moderator}', NOVICE_USER = '{editedUser.NoviceUser}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
                  $"WHERE ID = {editedUser.Id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.ExecuteCommand(query, paramters, values);
        return result;
      }
    }

    public void UpdateByModeratorWithLog(int id, UserAccount userAccount, string currentUsername)
    {
      UpdateByModerator(id, userAccount);
      var newLog = new NewLogViewModel(currentUsername, Operation.Alteration.GetEnumDescription(), id, ReferenceTable.UserAccount.GetEnumDescription());
      newLog.Description = "de usuário";
      _logRepository.Add(newLog);
    }
  }
}