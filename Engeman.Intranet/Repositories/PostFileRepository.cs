using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class PostFileRepository : IPostFileRepository
  {
    public List<PostFile> GetByPostId(int postId)
    {
      var files = new List<PostFile>();
      var query = $"SELECT * FROM POSTFILE WHERE POST_ID = {postId}";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var file = new PostFile
        {
          Id = Convert.ToInt32(result.Rows[i]["id"]),
          Active = Convert.ToBoolean(result.Rows[i]["Active"]),
          Name = result.Rows[i]["Name"].ToString(),
          BinaryData = (byte[])result.Rows[i]["Binary_Data"],
          ChangeDate = (DateTime)result.Rows[i]["Change_Date"]
        };
        files.Add(file);
      }

      return files;
    }

    public void Add(int postId, List<NewPostFileViewModel> files)
    {
      string query;
      string[] paramters = { "BinaryData;byte" };

      using StaticQuery sq = new();
      for (int i = 0; i < files.Count; i++)
      {
        object[] values = { files[i].BinaryData };
        query = $"INSERT INTO POSTFILE(NAME, BINARY_DATA, POST_ID) VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {postId})";
        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public void Delete(int id)
    {
      var query = $"DELETE FROM POSTFILE WHERE ID = {id}";

      using StaticQuery sq = new();
      sq.ExecuteCommand(query);
    }
  }
}
