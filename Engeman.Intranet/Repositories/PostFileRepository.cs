﻿using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostFileRepository : IPostFileRepository
  {
    public List<PostFile> GetByPostId(int postId)
    {
      List<PostFile> files = new List<PostFile>();
      var query = $"SELECT * FROM POSTFILE WHERE POST_ID = {postId}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          PostFile file = new PostFile();
          file.Id = Convert.ToInt32(result.Rows[i]["id"]);
          file.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
          file.Name = result.Rows[i]["Name"].ToString();
          file.BinaryData = (byte[])result.Rows[i]["Binary_Data"];
          file.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
          files.Add(file);
        }
        return files;
      }
    }

    public void Add(int postId, List<NewPostFileViewModel> files)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query = "";
        string[] paramters = { "BinaryData;byte" };

        for (int i = 0; i < files.Count; i++)
        {
          Object[] values = { files[i].BinaryData };

          query =
          "INSERT " +
          "INTO POSTFILE(NAME, BINARY_DATA, POST_ID) " +
          $"VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {postId}) ";

          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public void Delete(int id)
    {
      var query = $"DELETE FROM POSTFILE WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        sq.ExecuteCommand(query);
      }
    }
  }
}
