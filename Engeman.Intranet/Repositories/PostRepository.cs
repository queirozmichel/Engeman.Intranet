using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class PostRepository : IPostRepository
  {
    public List<Post> Get()
    {
      var posts = new List<Post>();
      var query = $"SELECT * FROM POST";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Post>();

      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var post = new Post
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]),
            Subject = result.Rows[i]["Subject"].ToString(),
            Description = result.Rows[i]["Description"].ToString(),
            CleanDescription = result.Rows[i]["Clean_Description"].ToString(),
            Keywords = result.Rows[i]["Keywords"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            PostType = Convert.ToChar(result.Rows[i]["Post_Type"]),
            Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          posts.Add(post);
        }

        return posts;
      }
    }

    public List<PostGridViewModel> GetPostsGrid(UserAccount user, string searchPhrase)
    {
      var posts = new List<PostGridViewModel>();
      bool revised;
      bool restricted;
      int authorPostId;
      bool moderator;
      string query;

      if (string.IsNullOrWhiteSpace(searchPhrase))
      {
        query = $"SELECT POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.KEYWORDS, UA.ID AS USER_ACCOUNT_ID, " +
                $"POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.DESCRIPTION as DEPARTMENT FROM POST LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
                $"INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID WHERE POST.ACTIVE = 1 " +
                $"GROUP BY POST.ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.KEYWORDS, UA.ID, POST.POST_TYPE, POST.CHANGE_DATE, " +
                $"UA.NAME, UA.MODERATOR, D.ID, D.DESCRIPTION";
      }
      else
      {
        query = $"SELECT POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.KEYWORDS, UA.ID AS USER_ACCOUNT_ID, " +
                $"POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.DESCRIPTION as DEPARTMENT, KEY_TBL.RANK " +
                $"FROM POST LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
                $"INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID " +
                $"INNER JOIN {Constants.SearchCondition.Replace("#SearchPhrase#", searchPhrase)} ON POST.ID = KEY_TBL.[KEY] " +
                $"WHERE POST.ACTIVE = 1 AND KEY_TBL.RANK >= {Constants.Rank} " +
                $"ORDER BY KEY_TBL.RANK DESC";
      }

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
        authorPostId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
        restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
        moderator = user.Moderator;
        if (revised == false && authorPostId != user.Id && moderator == false) continue;
        if (restricted == true)
        {
          query = $"SELECT COUNT(*) FROM POSTRESTRICTION WHERE POST_ID = {result.Rows[i]["Post_Id"]} AND DEPARTMENT_ID = {user.DepartmentId}";
          var aux = sq.GetDataToInt(query);
          //se não fizer parte do setor que há na tabela de restrição e se não for o autor da postagem
          if (aux == 0 && authorPostId != user.Id && moderator == false) continue;
        }
        var postGrid = new PostGridViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["Post_Id"]),
          Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]),
          Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
          Subject = result.Rows[i]["Subject"].ToString(),
          ChangeDate = result.Rows[i]["Change_Date"].ToString(),
          PostType = Convert.ToChar(result.Rows[i]["Post_Type"]),
          Keywords = result.Rows[i]["Keywords"].ToString(),
          UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
          Department = result.Rows[i]["Department"].ToString(),
          UserAccountName = result.Rows[i]["Name"].ToString()
        };
        posts.Add(postGrid);
      }
      return posts;
    }

    public List<Post> GetByUserAccountId(int userAccountId)
    {

      var posts = new List<Post>();
      var query = $"SELECT * FROM POST WHERE USER_ACCOUNT_ID = {userAccountId}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Post>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var post = new Post
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]),
            Subject = result.Rows[i]["Subject"].ToString(),
            Description = result.Rows[i]["Description"].ToString(),
            CleanDescription = result.Rows[i]["Clean_Description"].ToString(),
            Keywords = result.Rows[i]["Keywords"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            PostType = Convert.ToChar(result.Rows[i]["Post_Type"]),
            Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          posts.Add(post);
        }

        return posts;
      }
    }

    public List<Post> GetByUsername(string username)
    {
      var posts = new List<Post>();
      var query = $"SELECT * FROM POST as P INNER JOIN USERACCOUNT as U ON P.USER_ACCOUNT_ID =  U.ID WHERE U.USERNAME = '{username}'";

      using StaticQuery sq = new StaticQuery();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Post>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var post = new Post
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]),
            Subject = result.Rows[i]["Subject"].ToString(),
            Description = result.Rows[i]["Description"].ToString(),
            CleanDescription = result.Rows[i]["Clean_Description"].ToString(),
            Keywords = result.Rows[i]["Keywords"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            PostType = Convert.ToChar(result.Rows[i]["Post_Type"]),
            Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          posts.Add(post);
        }

        return posts;
      }
    }

    public string GetSubjectById(int id)
    {
      var query = $"SELECT SUBJECT FROM POST WHERE ID = {id}";

      using StaticQuery sq = new StaticQuery();
      var result = sq.GetDataSet(query).Tables[0].Rows[0];

      return result["subject"].ToString();
    }

    public Post GetById(int id)
    {
      var query = $"SELECT * FROM POST WHERE ID = {id}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0].Rows[0];

      var post = new Post
      {
        Id = Convert.ToInt32(result["Id"]),
        Active = Convert.ToBoolean(result["Active"]),
        Restricted = Convert.ToBoolean(result["Restricted"]),
        Subject = result["Subject"].ToString(),
        Description = result["Description"].ToString(),
        CleanDescription = result["Clean_Description"].ToString(),
        Keywords = result["Keywords"].ToString(),
        UserAccountId = Convert.ToInt32(result["User_Account_Id"]),
        PostType = Convert.ToChar(result["Post_Type"].ToString()),
        Revised = Convert.ToBoolean(result["Revised"]),
        ChangeDate = (DateTime)result["Change_Date"]
      };

      return post;
    }

    public List<PostGridViewModel> GetWithUnrevisedComments()
    {
      var posts = new List<PostGridViewModel>();

      string query = $"SELECT DISTINCT POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.KEYWORDS, UA.ID AS USER_ACCOUNT_ID, " +
                     $"POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.ID as DEPARTMENT_ID, D.DESCRIPTION as DEPARTMENT " +
                     $"FROM POST INNER JOIN COMMENT AS C ON C.POST_ID = POST.ID LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
                     $"INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID WHERE C.REVISED = 0";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var postGrid = new PostGridViewModel
        {
          Id = Convert.ToInt32(result.Rows[i]["Post_Id"]),
          Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]),
          Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
          Subject = result.Rows[i]["Subject"].ToString(),
          ChangeDate = result.Rows[i]["Change_Date"].ToString(),
          PostType = Convert.ToChar(result.Rows[i]["Post_Type"]),
          Keywords = result.Rows[i]["Keywords"].ToString(),
          UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
          Department = result.Rows[i]["Department"].ToString(),
          UserAccountName = result.Rows[i]["Name"].ToString(),
          UnrevisedComments = true
        };
        posts.Add(postGrid);
      }

      return posts;
    }

    public void Add(NewPostViewModel post, string currentUsername)
    {
      string[] paramters = { "BinaryData;byte" };
      var query = $"INSERT INTO POST (RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USER_ACCOUNT_ID, POST_TYPE, REVISED) OUTPUT INSERTED.ID " +
                  $"VALUES ('{post.Restricted}', '{post.Subject.Replace("'", "''")}', N'{post.Description.Replace("'", "''")}', '{post.CleanDescription.Replace("'", "''")}', '{post.Keywords}', {post.UserAccountId}, " +
                  $"'{post.PostType}', '{post.Revised}')";

      using StaticQuery sq = new();
      var outputPostId = sq.GetDataToInt(query);

      for (int i = 0; i < post.Files.Count; i++)
      {
        object[] values = { post.Files[i].BinaryData };
        query = $"INSERT INTO POSTFILE(NAME, BINARY_DATA, POST_ID) VALUES('{post.Files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {outputPostId})";
        sq.ExecuteCommand(query, paramters, values);
      }

      if (post.DepartmentsList != null)
      {
        for (int i = 0; i < post.DepartmentsList.Count; i++)
        {
          query = $"INSERT INTO POSTRESTRICTION(POST_ID, DEPARTMENT_ID) VALUES({outputPostId},{post.DepartmentsList[i]})";
          sq.ExecuteCommand(query);
        }
      }

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('I', "POS", outputPostId, "POST", currentUsername);
      }
    }

    public void Update(int id, PostEditViewModel post, string currentUsername)
    {
      var update = $"UPDATE POST SET RESTRICTED = '{post.Restricted}', SUBJECT = '{post.Subject.Replace("'", "''")}', DESCRIPTION = N'{post.Description.Replace("'", "''")}', " +
                   $"CLEAN_DESCRIPTION = '{post.CleanDescription.Replace("'", "''")}', KEYWORDS = '{post.Keywords}', POST_TYPE = '{post.PostType}', REVISED = '{post.Revised}' " +
                   $"WHERE ID = {post.Id}";
      var delete = $"DELETE FROM POSTRESTRICTION WHERE POST_ID = {post.Id}";

      using StaticQuery sq = new();
      sq.ExecuteCommand(update);

      if (post.Restricted == false) sq.ExecuteCommand(delete);
      else
      {
        sq.ExecuteCommand(delete);
        for (int i = 0; i < post.DepartmentsList.Count; i++)
        {
          var insert = $"INSERT INTO POSTRESTRICTION(POST_ID, DEPARTMENT_ID) VALUES({post.Id},{post.DepartmentsList[i]})";
          sq.ExecuteCommand(insert);
        }
      }

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('U', "POS", id, "POST", currentUsername);
      }
    }

    public void Delete(int id, string currentUsername)
    {
      string[] paramters = { };
      object[] values = { id };
      var query = $"DELETE FROM POST WHERE ID = {id}";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query, paramters, values);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('D', "POS", id, "POST", currentUsername);
      }
    }   

    public void Aprove(int id, string currentUsername)
    {
      string query = $"UPDATE POST SET REVISED = 'true' WHERE ID = '{id}'";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('A', "POS", id, "POST", currentUsername);
      }
    }

    public int CountByUsername(string username)
    {
      var query = $"SELECT COUNT(*) FROM POST AS P INNER JOIN USERACCOUNT AS U ON P.USER_ACCOUNT_ID = U.ID WHERE U.USERNAME = '{username}'";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }

    public int CountByUserId(int userId)
    {
      var query = $"SELECT COUNT(*) FROM POST WHERE USER_ACCOUNT_ID = {userId}";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }

    public int CountByPostType(char postType)
    {
      var query = $"SELECT COUNT(*) FROM POST WHERE POST_TYPE = '{postType}'";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }
  }
}