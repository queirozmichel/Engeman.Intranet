using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostCommentFileRepository : IPostCommentFileRepository
  {
    public IEnumerable<PostCommentFile> GetFilesByPostCommentId(int id)
    {
      List<PostCommentFile> files = new List<PostCommentFile>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT * " +
        $"FROM POST_COMMENT_FILE " +
        $"WHERE POST_COMMENT_ID = {id} ";

        var result = sq.GetDataSet(query).Tables[0].Rows;

        for (int i = 0; i < result.Count; i++)
        {
          var file = new PostCommentFile();
          file.Id = Convert.ToInt32(result[i]["Id"]);
          file.Active = Convert.ToChar(result[i]["Active"]);
          file.FileType = Convert.ToChar(result[i]["File_Type"]);
          file.Name = result[i]["Name"].ToString();
          file.Description = result[i]["Description"].ToString();
          file.BinaryData = (byte[])result[i]["Binary_Data"];
          file.PostCommentId = Convert.ToInt32(result[i]["Post_Comment_Id"]);
          file.ChangeDate = (DateTime)result[i]["ChangeDate"];
          files.Add(file);
        }
        return files;
      }
    }
  }
}
