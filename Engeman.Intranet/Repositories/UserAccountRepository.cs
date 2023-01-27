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
        Moderator = Convert.ToBoolean(result.Rows[0]["Moderator"]),
        NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"]),
        CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]),
        EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]),
        DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]),
        EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]),
        DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]),
        ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString())
      };

      return userAccount;
    }

    public UserAccount GetByUsername(string username)
    {
      var query = $"SELECT UA.ID,UA.ACTIVE,NAME,USERNAME,D.ID AS DEPARTMENT_ID,D.DESCRIPTION AS DEPARTMENT_DESCRIPTION,EMAIL," +
                  $"PHOTO,UA.DESCRIPTION AS USERDESCRIPTION, UA.CREATE_POST, UA.EDIT_OWNER_POST, UA.DELETE_OWNER_POST, UA.EDIT_ANY_POST, " +
                  $"UA.DELETE_ANY_POST, UA.MODERATOR, UA.NOVICE_USER, UA.CHANGE_DATE FROM USERACCOUNT UA INNER JOIN DEPARTMENT D " +
                  $"ON UA.DEPARTMENT_ID = D.ID WHERE USERNAME = '{username.ToLower()}'";

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
        CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]),
        EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]),
        DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]),
        EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]),
        DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]),
        Moderator = Convert.ToBoolean(result.Rows[0]["moderator"]),
        NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"]),
        ChangeDate = Convert.ToDateTime(result.Rows[0]["change_date"].ToString())
      };
      return userAccount;
    }

    public string GetUsernameById(int id)
    {
      var query = $"SELECT USERNAME FROM USERACCOUNT WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataToString(query);

      return result;
    }

    public UserPermissionsViewModel GetUserPermissionsByUsername(string username)
    {
      var query = $"SELECT CREATE_POST, EDIT_OWNER_POST, DELETE_OWNER_POST, EDIT_ANY_POST, DELETE_ANY_POST, MODERATOR, NOVICE_USER " +
                  $"FROM USERACCOUNT WHERE USERNAME = '{username}'";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      var userPermissions = new UserPermissionsViewModel
      {
        CreatePost = Convert.ToBoolean(result.Rows[0]["create_post"]),
        EditOwnerPost = Convert.ToBoolean(result.Rows[0]["edit_owner_post"]),
        DeleteOwnerPost = Convert.ToBoolean(result.Rows[0]["delete_owner_post"]),
        EditAnyPost = Convert.ToBoolean(result.Rows[0]["edit_any_post"]),
        DeleteAnyPost = Convert.ToBoolean(result.Rows[0]["delete_any_post"]),
        Moderator = Convert.ToBoolean(result.Rows[0]["moderator"]),
        NoviceUser = Convert.ToBoolean(result.Rows[0]["novice_user"])
      };

      return userPermissions;
    }

    public List<UserGridViewModel> GetUsersGrid()
    {
      var users = new List<UserGridViewModel>();
      var query = $"SELECT U.ID, U.ACTIVE, U.NAME, U.USERNAME, D.DESCRIPTION AS DEPARTMENT, U.MODERATOR, U.NOVICE_USER FROM USERACCOUNT AS U INNER JOIN DEPARTMENT AS D " +
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
          Novice = Convert.ToBoolean(result.Rows[i]["novice_user"]),
          Moderator = Convert.ToBoolean(result.Rows[i]["moderator"])
        };
        users.Add(user);
      }

      return users;
    }

    public int Add(NewUserViewModel user)
    {
      var query = $"INSERT INTO USERACCOUNT (NAME, USERNAME, EMAIL, DEPARTMENT_ID, MODERATOR, NOVICE_USER) OUTPUT INSERTED.ID VALUES ('{user.Name}', '{user.Username}', " +
                  $"'{user.Email}', {user.DepartmentId}, {user.Moderator}, {user.NoviceUser})";

      using StaticQuery sq = new();
      var outputUserId = sq.GetDataToInt(query);

      return outputUserId;
    }

    public void AddWithLog(NewUserViewModel user, string currentUsername)
    {
      var outputUserId = Add(user);

      var newLog = new NewLogViewModel(currentUsername, Operation.Inclusion.GetEnumDescription(), outputUserId, ReferenceTable.UserAccount.GetEnumDescription())
      {
        Description = "de usuário"
      };

      _logRepository.Add(newLog);
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

    public void UpdateByModerator(int id, UserAccount editedUser)
    {
      string[] paramters = { "Photo;byte" };
      object[] values = { editedUser.Photo };
      var query = $"UPDATE USERACCOUNT SET ACTIVE = '{editedUser.Active}', NAME = '{editedUser.Name}', USERNAME = '{editedUser.Username}', EMAIL = '{editedUser.Email}', " +
                  $"DEPARTMENT_ID = {editedUser.DepartmentId}, DESCRIPTION = '{editedUser.Description}', CREATE_POST = '{editedUser.CreatePost}', " +
                  $"EDIT_OWNER_POST = '{editedUser.EditOwnerPost}', DELETE_OWNER_POST = '{editedUser.DeleteOwnerPost}', EDIT_ANY_POST = '{editedUser.EditAnyPost}', " +
                  $"DELETE_ANY_POST = '{editedUser.DeleteAnyPost}', MODERATOR = '{editedUser.Moderator}', NOVICE_USER = '{editedUser.NoviceUser}', PHOTO = CONVERT(VARBINARY(MAX),@Photo) " +
                  $"WHERE ID = {editedUser.Id}";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query, paramters, values);
    }

    public void UpdateByModeratorWithLog(int id, UserAccount userAccount, string currentUsername)
    {
      UpdateByModerator(id, userAccount);

      var newLog = new NewLogViewModel(currentUsername, Operation.Alteration.GetEnumDescription(), id, ReferenceTable.UserAccount.GetEnumDescription())
      {
        Description = "de usuário"
      };

      _logRepository.Add(newLog);
    }

    public void Delete(int userId)
    {
      string query = $"DELETE FROM USERACCOUNT WHERE ID = {userId}";

      using StaticQuery sq = new();
      var result = sq.ExecuteCommand(query);
    }

    public void DeleteWithLog(int id, string currentUsername)
    {
      Delete(id);

      var newLog = new NewLogViewModel(currentUsername, Operation.Exclusion.GetEnumDescription(), id, ReferenceTable.UserAccount.GetEnumDescription())
      {
        Description = "de usuário"
      };

      _logRepository.Add(newLog);
    }
  }
}