using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostCommentRepository : IPostCommentRepository
  {
    public void AddPostComment(NewCommentViewModel newComment)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        //É inserido o caracter 'N' antes da descrição para codificar o emoji corretamente no banco de dados
        var query =
        $"INSERT INTO " +
        $"POST_COMMENT " +
        $"(DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USER_ACCOUNT_ID, DEPARTMENT_ID, POST_ID, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES(N'{newComment.Description}', '{newComment.CleanDescription}', '{newComment.Keywords}', " +
        $"'{newComment.UserAccountId}', '{newComment.DepartmentId}', '{newComment.PostId}', '{newComment.Revised}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < newComment.Files.Count; i++)
        {
          Object[] values = { newComment.Files[i].BinaryData };
          query =
          $"INSERT INTO " +
          $"POST_COMMENT_FILE " +
          $"(NAME, DESCRIPTION, BINARY_DATA, POST_COMMENT_ID) " +
          $"VALUES('{newComment.Files[i].Name}', '{newComment.Files[i].Description}', Convert(VARBINARY(MAX),@BinaryData), '{outputPostId}') ";

          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public List<PostComment> GetPostCommentsByPostId(int postId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        List<PostComment> postComments = new List<PostComment>();

        var query =
        $"SELECT * " +
        $"FROM POST_COMMENT " +
        $"WHERE POST_ID = {postId} ";

        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          return new List<PostComment>();
        }
        else
        {
          for (int i = 0; i < result.Rows.Count; i++)
          {
            PostComment postComment = new PostComment();
            postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
            postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
            postComment.Description = result.Rows[i]["Description"].ToString();
            postComment.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
            postComment.Keywords = result.Rows[i]["Keywords"].ToString();
            postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
            postComment.DepartmentId = Convert.ToInt32(result.Rows[i]["Department_Id"]);
            postComment.PostId = postId;
            postComment.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
            postComments.Add(postComment);
          }
          return postComments;
        }
      }
    }

    public bool DeletePostCommentById(int id)
    {
      bool result = true;
      var query =
      $"DELETE " +
      $"FROM POST_COMMENT " +
      $"WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var aux = sq.ExecuteCommand(query);
        if (aux != 1)
        {
          result = false;
        }
        return result;
      }
    }

    public List<PostComment> GetPostCommentsByRestriction(UserAccount user, int postId)
    {
      List<PostComment> postComments = new List<PostComment>();

      using (StaticQuery sq = new StaticQuery())
      {
        string query =
        $"SELECT " +
        $"ID, ACTIVE, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USER_ACCOUNT_ID, DEPARTMENT_ID, CHANGE_DATE, REVISED " +
        $"FROM POST_COMMENT " +
        $"WHERE POST_ID = '{postId}'";

        var result = sq.GetDataSet(query).Tables[0];

        bool revised;
        int authorCommentId;
        bool moderator;

        for (int i = 0; i < result.Rows.Count; i++)
        {
          revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          authorCommentId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          moderator = user.Moderator;

          if (revised == false && authorCommentId != user.Id && moderator == false)
          {
            continue;
          }

          PostComment postComment = new PostComment();
          postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
          postComment.Description = result.Rows[i]["Description"].ToString();
          postComment.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postComment.Keywords = result.Rows[i]["Keywords"].ToString();
          postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postComment.DepartmentId = Convert.ToInt32(result.Rows[i]["Department_Id"]);
          postComment.PostId = postId;
          postComment.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postComment.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
          postComments.Add(postComment);
        }
        return postComments;
      }
    }

    public PostComment GetPostCommentById(int commentId)
    {
      PostComment comment = new PostComment();

      using (StaticQuery sq = new StaticQuery())
      {
        string query =
        $"SELECT * " +
        $"FROM POST_COMMENT " +
        $"WHERE ID = '{commentId}'";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        comment.Id = Convert.ToInt32(result["id"]);
        comment.Active = Convert.ToBoolean(result["active"]);
        comment.Description = Convert.ToString(result["description"]);
        comment.CleanDescription = Convert.ToString(result["clean_description"]);
        comment.Keywords = Convert.ToString(result["keywords"]);
        comment.UserAccountId = Convert.ToInt32(result["user_account_id"]);
        comment.DepartmentId = Convert.ToInt32(result["department_id"]);
        comment.PostId = Convert.ToInt32(result["post_id"]);
        comment.ChangeDate = (DateTime)result["change_date"];
        comment.Revised = Convert.ToBoolean(result["revised"]);

        return comment;
      }
    }

    public bool UpdatePostCommentById(int id, PostComment comment)
    {
      string query =
      $"UPDATE " +
      $"POST_COMMENT " +
      $"SET ACTIVE = '{comment.Active}', DESCRIPTION = N'{comment.Description}', CLEAN_DESCRIPTION = '{comment.CleanDescription}', " +
      $"KEYWORDS = '{comment.Keywords}', REVISED = '{comment.Revised}' " +
      $"WHERE ID = '{id}'";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.ExecuteCommand(query));

        return result;
      }
    }

    public bool UpdatePostCommentById(int id, PostComment comment, List<CommentFile> files)
    {
      string update;

      using (StaticQuery sq = new StaticQuery())
      {
        UpdatePostCommentById(id, comment);

        for (int i = 0; i < files.Count; i++)
        {
          update =
          $"UPDATE " +
          $"POST_COMMENT_FILE " +
          $"SET DESCRIPTION = N'{comment.Description}' " +
          $"WHERE POST_COMMENT_ID = {id}";

          sq.ExecuteCommand(update);
        }
      }
      return true;
    }

    public List<PostComment> GetUnrevisedComments()
    {
      string query =
      $"SELECT * " +
      $"FROM POST_COMMENT " +
      $"WHERE REVISED = 0 ";

      using (StaticQuery sq = new StaticQuery())
      {
        List<PostComment> comments = new List<PostComment>();

        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          PostComment comment = new PostComment();

          comment.Id = Convert.ToInt32(result.Rows[i]["id"]);
          comment.Active = Convert.ToBoolean(result.Rows[i]["active"]);
          comment.Description = Convert.ToString(result.Rows[i]["description"]);
          comment.CleanDescription = Convert.ToString(result.Rows[i]["clean_description"]);
          comment.Keywords = Convert.ToString(result.Rows[i]["keywords"]);
          comment.UserAccountId = Convert.ToInt32(result.Rows[i]["user_account_id"]);
          comment.DepartmentId = Convert.ToInt32(result.Rows[i]["department_id"]);
          comment.PostId = Convert.ToInt32(result.Rows[i]["post_id"]);
          comment.ChangeDate = (DateTime)result.Rows[i]["change_date"];
          comment.Revised = Convert.ToBoolean(result.Rows[i]["revised"]);

          comments.Add(comment);
        }

        return comments;
      }
    }

    public List<PostComment> GetPostCommentsByUserId(int userId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        List<PostComment> postComments = new List<PostComment>();

        var query =
        $"SELECT * " +
        $"FROM POST_COMMENT " +
        $"WHERE USER_ACCOUNT_ID = {userId} ";

        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          return new List<PostComment>();
        }
        else
        {
          for (int i = 0; i < result.Rows.Count; i++)
          {
            PostComment postComment = new PostComment();
            postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
            postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
            postComment.Description = result.Rows[i]["Description"].ToString();
            postComment.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
            postComment.Keywords = result.Rows[i]["Keywords"].ToString();
            postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
            postComment.DepartmentId = Convert.ToInt32(result.Rows[i]["Department_Id"]);
            postComment.PostId = postComment.PostId;
            postComment.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
            postComments.Add(postComment);
          }
          return postComments;
        }
      }
    }
  }
}
