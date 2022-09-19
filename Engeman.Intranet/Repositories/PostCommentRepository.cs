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
        $"(DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_ID) " +
        $"VALUES(N'{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
        $"'{postComment.UserAccountId}', '{postComment.DepartmentId}', '{postComment.PostId}') ";

        sq.ExecuteCommand(query);
      }
    }

    public void AddPostComment(PostComment postComment, List<Archive> archives)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        var query =
        $"INSERT INTO " +
        $"POST_COMMENT " +
        $"(DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_ID) OUTPUT INSERTED.ID " +
        $"VALUES(N'{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
        $"'{postComment.UserAccountId}', '{postComment.DepartmentId}', '{postComment.PostId}') ";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < archives.Count; i++)
        {
          Object[] values = { archives[i].BinaryData };
          query =
          $"INSERT INTO " +
          $"POST_COMMENT_FILE " +
          $"(NAME, DESCRIPTION, BINARY_DATA, POST_COMMENT_ID) " +
          $"VALUES('{archives[i].Name}', '{archives[i].Description}', Convert(VARBINARY(MAX),@BinaryData), '{outputPostId}') ";

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
  }
}
