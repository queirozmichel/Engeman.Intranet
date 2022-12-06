﻿using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostCommentFileRepository : IPostCommentFileRepository
  {
    public List<CommentFile> GetFilesByPostCommentId(int id)
    {
      List<CommentFile> files = new List<CommentFile>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT * " +
        $"FROM COMMENTFILE " +
        $"WHERE COMMENT_ID = {id} ";

        var result = sq.GetDataSet(query).Tables[0].Rows;

        for (int i = 0; i < result.Count; i++)
        {
          var file = new CommentFile();
          file.Id = Convert.ToInt32(result[i]["Id"]);
          file.Active = Convert.ToBoolean(result[i]["Active"]);
          file.Name = result[i]["Name"].ToString();
          file.BinaryData = (byte[])result[i]["Binary_Data"];
          file.CommentId = Convert.ToInt32(result[i]["Comment_Id"]);
          file.ChangeDate = (DateTime)result[i]["Change_Date"];
          files.Add(file);
        }
        return files;
      }
    }

    public void DeleteFileById(int id)
    {
      string query =
      $"DELETE " +
      $"FROM COMMENTFILE " +
      $"WHERE ID = {id} ";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.GetDataToInt(query));
      }
    }

    public void AddFilesToComment(int id, List<CommentFile> files)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string query = "";
        string[] paramters = { "BinaryData;byte" };

        for (int i = 0; i < files.Count; i++)
        {
          Object[] values = { files[i].BinaryData };
          query =
          $"INSERT INTO " +
          $"COMMENTFILE " +
          $"(NAME, BINARY_DATA, COMMENT_ID) " +
          $"VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {id}) ";

          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }
  }
}
