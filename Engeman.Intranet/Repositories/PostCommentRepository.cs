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
        $"VALUES('{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
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
        $"VALUES('{postComment.Description}', '{postComment.CleanDescription}', '{postComment.Keywords}', " +
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
  }
}
