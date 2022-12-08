using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class CommentFileRepository : ICommentFileRepository
  {
    public List<CommentFile> GetByCommentId(int id)
    {
      List<CommentFile> files = new List<CommentFile>();
      var query = $"SELECT * FROM COMMENTFILE WHERE COMMENT_ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          var file = new CommentFile();
          file.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          file.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
          file.Name = result.Rows[i]["Name"].ToString();
          file.BinaryData = (byte[])result.Rows[i]["Binary_Data"];
          file.CommentId = Convert.ToInt32(result.Rows[i]["Comment_Id"]);
          file.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
          files.Add(file);
        }
        return files;
      }
    }

    public void Add(int id, List<CommentFile> files)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string query = "";
        string[] paramters = { "BinaryData;byte" };

        for (int i = 0; i < files.Count; i++)
        {
          Object[] values = { files[i].BinaryData };
          query = $"INSERT INTO COMMENTFILE(NAME, BINARY_DATA, COMMENT_ID) VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {id}) ";
          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public void Delete(int id)
    {
      string query = $"DELETE FROM COMMENTFILE WHERE ID = {id}";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.GetDataToInt(query));
      }
    }
  }
}