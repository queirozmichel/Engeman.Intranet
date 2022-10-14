using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostCommentRepository : IPostCommentRepository
  {
    public void AddPostComment(PostComment postComment)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"INSERT INTO " +
        $"POST_COMMENT " +
        $"(DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_ID, REVISED) " +
        $"VALUES(N'{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
        $"'{postComment.UserAccountId}', '{postComment.DepartmentId}', '{postComment.PostId}', '{postComment.Revised}')";

        sq.ExecuteCommand(query);
      }
    }

    public void AddPostComment(PostComment postComment, List<PostFile> files)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        var query =
        $"INSERT INTO " +
        $"POST_COMMENT " +
        $"(DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_ID, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES(N'{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
        $"'{postComment.UserAccountId}', '{postComment.DepartmentId}', '{postComment.PostId}', '{postComment.Revised}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < files.Count; i++)
        {
          Object[] values = { files[i].BinaryData };
          query =
          $"INSERT INTO " +
          $"POST_COMMENT_FILE " +
          $"(NAME, DESCRIPTION, BINARY_DATA, POST_COMMENT_ID) " +
          $"VALUES('{files[i].Name}', '{files[i].Description}', Convert(VARBINARY(MAX),@BinaryData), '{outputPostId}') ";

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
            postComment.Active = Convert.ToChar(result.Rows[i]["Active"]);
            postComment.Description = result.Rows[i]["Description"].ToString();
            postComment.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
            postComment.Keywords = result.Rows[i]["Keywords"].ToString();
            postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
            postComment.DepartmentId = Convert.ToInt32(result.Rows[i]["Department_Id"]);
            postComment.PostId = postId;
            postComment.ChangeDate = (DateTime)result.Rows[i]["ChangeDate"];
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
        $"ID, ACTIVE, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, CHANGEDATE, REVISED " +
        $"FROM POST_COMMENT " +
        $"WHERE POST_ID = '{postId}'";

        var result = sq.GetDataSet(query).Tables[0];

        bool revised;
        int authorCommentId;
        bool moderator;

        for (int i = 0; i < result.Rows.Count; i++)
        {
          revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          authorCommentId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          moderator = user.Moderator;

          if (revised == false && authorCommentId != user.Id && moderator == false)
          {
            continue;
          }

          PostComment postComment = new PostComment();
          postComment.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          postComment.Active = Convert.ToChar(result.Rows[i]["Active"]);
          postComment.Description = result.Rows[i]["Description"].ToString();
          postComment.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postComment.Keywords = result.Rows[i]["Keywords"].ToString();
          postComment.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          postComment.DepartmentId = Convert.ToInt32(result.Rows[i]["Department_Id"]);
          postComment.PostId = postId;
          postComment.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postComment.ChangeDate = (DateTime)result.Rows[i]["ChangeDate"];
          postComments.Add(postComment);
        }
        return postComments;
      }
    }

    public PostComment GetPostCommentById(int postId)
    {
      PostComment comment = new PostComment();

      using (StaticQuery sq = new StaticQuery())
      {
        string query =
        $"SELECT * " +
        $"FROM POST_COMMENT " +
        $"WHERE ID = '{postId}'";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        comment.Id = Convert.ToInt32(result["id"]);
        comment.Active = Convert.ToChar(result["active"]);
        comment.Description = Convert.ToString(result["description"]);
        comment.CleanDescription = Convert.ToString(result["clean_description"]);
        comment.Keywords = Convert.ToString(result["keywords"]);
        comment.UserAccountId = Convert.ToInt32(result["useraccount_id"]);
        comment.DepartmentId = Convert.ToInt32(result["department_id"]);
        comment.PostId = postId;
        comment.ChangeDate = (DateTime)result["changedate"];
        comment.Revised = Convert.ToBoolean(result["revised"]);

        return comment;
      }
    }

    public bool UpdatePostCommentById(int id, PostComment comment)
    {
      string query =
      $"UPDATE " +
      $"POST_COMMENT " +
      $"SET ACTIVE = '{comment.Active}', DESCRIPTION = '{comment.Description}', CLEAN_DESCRIPTION = '{comment.CleanDescription}', " +
      $"KEYWORDS = '{comment.Keywords}', USERACCOUNT_ID = {comment.UserAccountId}, DEPARTMENT_ID = {comment.DepartmentId}, " +
      $"REVISED = '{comment.Revised}' " +
      $"WHERE ID = '{id}'";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.ExecuteCommand(query));

        return result;
      }
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
          comment.Active = Convert.ToChar(result.Rows[i]["active"]);
          comment.Description = Convert.ToString(result.Rows[i]["description"]);
          comment.CleanDescription = Convert.ToString(result.Rows[i]["clean_description"]);
          comment.Keywords = Convert.ToString(result.Rows[i]["keywords"]);
          comment.UserAccountId = Convert.ToInt32(result.Rows[i]["useraccount_id"]);
          comment.DepartmentId = Convert.ToInt32(result.Rows[i]["department_id"]);
          comment.PostId = Convert.ToInt32(result.Rows[i]["post_id"]);
          comment.ChangeDate = (DateTime)result.Rows[i]["changedate"];
          comment.Revised = Convert.ToBoolean(result.Rows[i]["revised"]);

          comments.Add(comment);
        }

        return comments;
      }
    }
  }
}
