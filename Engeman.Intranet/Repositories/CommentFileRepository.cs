using Engeman.Intranet.Library;
using Engeman.Intranet.Models;

namespace Engeman.Intranet.Repositories
{
  public class CommentFileRepository : ICommentFileRepository
  {
    public List<CommentFile> GetByCommentId(int commentId)
    {
      var files = new List<CommentFile>();
      var query = $"SELECT * FROM COMMENTFILE WHERE COMMENT_ID = {commentId}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];
      for (int i = 0; i < result.Rows.Count; i++)
      {
        var file = new CommentFile
        {
          Id = Convert.ToInt32(result.Rows[i]["Id"]),
          Name = result.Rows[i]["Name"].ToString(),
          BinaryData = (byte[])result.Rows[i]["Binary_Data"],
          CommentId = Convert.ToInt32(result.Rows[i]["Comment_Id"]),
          ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
        };
        files.Add(file);
      }

      return files;
    }

    public void Add(int commentId, List<CommentFile> files)
    {
      string query;
      string[] paramters = { "BinaryData;byte" };

      using StaticQuery sq = new();

      for (int i = 0; i < files.Count; i++)
      {
        object[] values = { files[i].BinaryData };
        query = $"INSERT INTO COMMENTFILE(NAME, BINARY_DATA, COMMENT_ID) VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {commentId}) ";
        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public void Delete(int[] ids)
    {
      using StaticQuery sq = new();

      for (int i = 0; i < ids.Length; i++)
      {
        var query = $"DELETE FROM COMMENTFILE WHERE ID = {ids[i]}";
        sq.ExecuteCommand(query);
      }
    }
  }
}