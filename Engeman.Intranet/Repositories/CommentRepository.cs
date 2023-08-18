using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class CommentRepository : ICommentRepository
  {
    public List<Comment> Get()
    {
      var comments = new List<Comment>();
      var query = $"SELECT * FROM COMMENT WHERE ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Comment>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var comment = new Comment
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Description = result.Rows[i]["Description"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"],
            PostId = Convert.ToInt32(result.Rows[i]["Post_Id"]),
          };
          comments.Add(comment);
        }

        return comments;
      }
    }

    public Comment GetById(int id)
    {
      string query = $"SELECT * FROM COMMENT WHERE ID = '{id}' AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      var comment = new Comment
      {
        Id = Convert.ToInt32(result.Rows[0]["id"]),
        Active = Convert.ToBoolean(result.Rows[0]["active"]),
        Description = Convert.ToString(result.Rows[0]["description"]),
        UserAccountId = Convert.ToInt32(result.Rows[0]["user_account_id"]),
        PostId = Convert.ToInt32(result.Rows[0]["post_id"]),
        ChangeDate = (DateTime)result.Rows[0]["change_date"],
        Revised = Convert.ToBoolean(result.Rows[0]["revised"])
      };

      return comment;
    }

    public List<Comment> GetByPostId(int postId)
    {
      var comments = new List<Comment>();
      var query = $"SELECT * FROM COMMENT WHERE POST_ID = {postId} AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Comment>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var comment = new Comment
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Description = result.Rows[i]["Description"].ToString(),
            PostId = postId,
            Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          comments.Add(comment);
        }

        return comments;
      }
    }

    public List<Comment> GetByUserAccountId(int userAccountId)
    {
      var comments = new List<Comment>();
      var query = $"SELECT * FROM COMMENT WHERE USER_ACCOUNT_ID = {userAccountId} AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Comment>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var comment = new Comment
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Description = result.Rows[i]["Description"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            PostId = Convert.ToInt32(result.Rows[i]["Post_Id"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          comments.Add(comment);
        }

        return comments;
      }
    }

    public List<Comment> GetUnrevisedComments()
    {
      var comments = new List<Comment>();
      string query = $"SELECT * FROM COMMENT WHERE REVISED = 0 AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var comment = new Comment
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Active = Convert.ToBoolean(result.Rows[i]["active"]),
          Description = Convert.ToString(result.Rows[i]["description"]),
          PostId = Convert.ToInt32(result.Rows[i]["post_id"]),
          ChangeDate = (DateTime)result.Rows[i]["change_date"],
          Revised = Convert.ToBoolean(result.Rows[i]["revised"])
        };
        comments.Add(comment);
      }

      return comments;
    }

    public List<Comment> GetByUserRestriction(UserAccount userAccount, int postId)
    {
      var comments = new List<Comment>();
      string query = $"SELECT ID, ACTIVE, DESCRIPTION, USER_ACCOUNT_ID, CHANGE_DATE, REVISED FROM COMMENT WHERE POST_ID = '{postId}' AND ACTIVE = 1";
      bool revised;
      int authorCommentId;
      bool moderator;

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
        authorCommentId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
        moderator = GlobalFunctions.IsModerator(userAccount.Id);

        if (revised == false && authorCommentId != userAccount.Id && moderator == false) continue;

        var comment = new Comment
        {
          Id = Convert.ToInt32(result.Rows[i]["Id"]),
          Active = Convert.ToBoolean(result.Rows[i]["Active"]),
          Description = result.Rows[i]["Description"].ToString(),
          UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
          PostId = postId,
          Revised = Convert.ToBoolean(result.Rows[i]["Revised"]),
          ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
        };
        comments.Add(comment);
      }

      return comments;
    }

    public List<Comment> GetByUsername(string username)
    {
      var comments = new List<Comment>();
      var query = $"SELECT * FROM COMMENT as C INNER JOIN USERACCOUNT as U ON C.USER_ACCOUNT_ID =  U.ID WHERE U.USERNAME = '{username}' AND C.ACTIVE = 1 AND U.ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      if (result.Rows.Count == 0) return new List<Comment>();
      else
      {
        for (int i = 0; i < result.Rows.Count; i++)
        {
          var comment = new Comment
          {
            Id = Convert.ToInt32(result.Rows[i]["Id"]),
            Active = Convert.ToBoolean(result.Rows[i]["Active"]),
            Description = result.Rows[i]["Description"].ToString(),
            UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]),
            PostId = Convert.ToInt32(result.Rows[i]["Post_Id"]),
            ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
          };
          comments.Add(comment);
        }

        return comments;
      }
    }

    public int GetPostIdById(int id)
    {
      string query = $"SELECT POST_ID FROM COMMENT WHERE ID = {id} AND ACTIVE = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataToInt(query);
      return result;
    }

    public int Add(NewCommentViewModel newComment, string currentUsername)
    {
      string[] paramters = { "BinaryData;byte" };
      //É inserido o caracter 'N' antes da descrição para codificar o emoji corretamente no banco de dados
      var query = $"INSERT INTO COMMENT(DESCRIPTION, CLEAN_DESCRIPTION, USER_ACCOUNT_ID, POST_ID, REVISED) OUTPUT INSERTED.ID " +
                  $"VALUES(N'{newComment.Description.Replace("'", "''")}', '{newComment.CleanDescription.Replace("'", "''")}', {newComment.UserAccountId}, {newComment.PostId}, '{newComment.Revised}')";

      using StaticQuery sq = new();
      var outputCommentId = sq.GetDataToInt(query);

      for (int i = 0; i < newComment.Files.Count; i++)
      {
        object[] values = { newComment.Files[i].BinaryData };
        query = $"INSERT INTO COMMENTFILE(NAME, BINARY_DATA, COMMENT_ID) VALUES('{newComment.Files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), '{outputCommentId}') ";
        sq.ExecuteCommand(query, paramters, values);
      }

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('I', "COM", outputCommentId, "COMMENT", currentUsername);
      }
      return outputCommentId;
    }

    public void Update(int id, Comment comment, string currentUsername)
    {
      string query = $"UPDATE COMMENT SET DESCRIPTION = N'{comment.Description.Replace("'", "''")}', " +
                     $"CLEAN_DESCRIPTION = '{comment.CleanDescription.Replace("'", "''")}', REVISED = '{comment.Revised}' WHERE ID = '{id}'";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('U', "COM", id, "COMMENT", currentUsername);
      }
    }

    public void Delete(int id, string currentUsername)
    {
      var query = $"DELETE FROM COMMENT WHERE ID = {id}";

      using StaticQuery sq = new();
      var aux = sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('D', "COM", id, "COMMENT", currentUsername);
      }
    }

    public void Aprove(int id, string currentUsername)
    {
      string query = $"UPDATE COMMENT SET REVISED = 'true' WHERE ID = '{id}'";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query);

      if (!string.IsNullOrEmpty(currentUsername))
      {
        GlobalFunctions.NewLog('A', "COM", id, "COMMENT", currentUsername);
      }
    }

    public int CountByUsername(string username)
    {
      var query = $"SELECT COUNT(*) FROM COMMENT AS C INNER JOIN USERACCOUNT AS U ON C.USER_ACCOUNT_ID = U.ID " +
                  $"WHERE U.USERNAME = '{username}' AND C.ACTIVE = 1 AND U.ACTIVE = 1";

      using StaticQuery sq = new StaticQuery();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }

    public int CountByUserId(int userId)
    {
      var query = $"SELECT COUNT(*) FROM COMMENT WHERE USER_ACCOUNT_ID = {userId} AND ACTIVE = 1";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }

    public int Count()
    {
      var query = $"SELECT COUNT(*) FROM COMMENT WHERE ACTIVE = 1";

      using StaticQuery sq = new();
      int result = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0][0]);

      return result;
    }
  }
}