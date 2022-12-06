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
        $"COMMENT " +
        $"(DESCRIPTION, USER_ACCOUNT_ID, POST_ID, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES(N'{newComment.Description}', {newComment.UserAccountId}, {newComment.PostId}, '{newComment.Revised}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < newComment.Files.Count; i++)
        {
          Object[] values = { newComment.Files[i].BinaryData };
          query =
          $"INSERT INTO " +
          $"COMMENTFILE " +
          $"(NAME, BINARY_DATA, COMMENT_ID) " +
          $"VALUES('{newComment.Files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), '{outputPostId}') ";

          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public List<Comment> GetPostCommentsByPostId(int postId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        List<Comment> postComments = new List<Comment>();

        var query =
        $"SELECT * " +
        $"FROM COMMENT " +
        $"WHERE POST_ID = {postId} ";

        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          return new List<Comment>();
        }
        else
        {
          for (int i = 0; i < result.Rows.Count; i++)
          {
            Comment postComment = new Comment();
            postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
            postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
            postComment.Description = result.Rows[i]["Description"].ToString();
            postComment.PostId = postId;
            postComment.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
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
      $"FROM COMMENT " +
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

    public List<Comment> GetPostCommentsByRestriction(UserAccount user, int postId)
    {
      List<Comment> postComments = new List<Comment>();

      using (StaticQuery sq = new StaticQuery())
      {
        string query =
        $"SELECT " +
        $"ID, ACTIVE, DESCRIPTION, USER_ACCOUNT_ID, CHANGE_DATE, REVISED " +
        $"FROM COMMENT " +
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

          Comment postComment = new Comment();
          postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
          postComment.Description = result.Rows[i]["Description"].ToString();
          postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postComment.PostId = postId;
          postComment.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postComment.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
          postComments.Add(postComment);
        }
        return postComments;
      }
    }

    public Comment GetPostCommentById(int commentId)
    {
      Comment comment = new Comment();

      using (StaticQuery sq = new StaticQuery())
      {
        string query =
        $"SELECT * " +
        $"FROM COMMENT " +
        $"WHERE ID = '{commentId}'";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        comment.Id = Convert.ToInt32(result["id"]);
        comment.Active = Convert.ToBoolean(result["active"]);
        comment.Description = Convert.ToString(result["description"]);
        comment.UserAccountId = Convert.ToInt32(result["user_account_id"]);
        comment.PostId = Convert.ToInt32(result["post_id"]);
        comment.ChangeDate = (DateTime)result["change_date"];
        comment.Revised = Convert.ToBoolean(result["revised"]);

        return comment;
      }
    }

    public bool UpdatePostCommentById(int id, Comment comment)
    {
      string query =
      $"UPDATE " +
      $"COMMENT " +
      $"SET ACTIVE = '{comment.Active}', DESCRIPTION = N'{comment.Description}', REVISED = '{comment.Revised}' " +
      $"WHERE ID = '{id}'";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.ExecuteCommand(query));

        return result;
      }
    }    

    public List<Comment> GetUnrevisedComments()
    {
      string query =
      $"SELECT * " +
      $"FROM COMMENT " +
      $"WHERE REVISED = 0 ";

      using (StaticQuery sq = new StaticQuery())
      {
        List<Comment> comments = new List<Comment>();

        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          Comment comment = new Comment();

          comment.Id = Convert.ToInt32(result.Rows[i]["id"]);
          comment.Active = Convert.ToBoolean(result.Rows[i]["active"]);
          comment.Description = Convert.ToString(result.Rows[i]["description"]);
          comment.PostId = Convert.ToInt32(result.Rows[i]["post_id"]);
          comment.ChangeDate = (DateTime)result.Rows[i]["change_date"];
          comment.Revised = Convert.ToBoolean(result.Rows[i]["revised"]);

          comments.Add(comment);
        }

        return comments;
      }
    }

    public List<Comment> GetPostCommentsByUserId(int userId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        List<Comment> postComments = new List<Comment>();

        var query =
        $"SELECT * " +
        $"FROM COMMENT " +
        $"WHERE USER_ACCOUNT_ID = {userId} ";

        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          return new List<Comment>();
        }
        else
        {
          for (int i = 0; i < result.Rows.Count; i++)
          {
            Comment postComment = new Comment();
            postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
            postComment.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
            postComment.Description = result.Rows[i]["Description"].ToString();
            postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
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
