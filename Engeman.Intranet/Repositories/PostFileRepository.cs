using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostFileRepository : IPostFileRepository
  {
    public List<PostFile> GetFilesByPostId(int postId)
    {
      List<PostFile> files = new List<PostFile>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT * " +
        $"FROM POSTFILE " +
        $"WHERE POST_ID = {postId}";

        var result = sq.GetDataSet(query).Tables[0].Rows;

        for (int i = 0; i < result.Count; i++)
        {
          PostFile file = new PostFile();
          file.Id = Convert.ToInt32(result[i]["id"]);
          file.Active = Convert.ToBoolean(result[i]["Active"]);
          file.Name = result[i]["Name"].ToString();
          file.BinaryData = (byte[])result[i]["Binary_Data"];
          file.ChangeDate = (DateTime)result[i]["Change_Date"];
          files.Add(file);
        }
        return files;
      }
    }
    public void DeleteFileById(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"DELETE " +
        $"FROM POSTFILE " +
        $"WHERE ID = {id}";

        sq.ExecuteCommand(query);
      }
    }
  }
}
