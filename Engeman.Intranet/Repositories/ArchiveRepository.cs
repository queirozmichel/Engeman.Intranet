using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class ArchiveRepository : IArchiveRepository
  {
    public List<Archive> GetArchiveByPostId(int postId)
    {
      List<Archive> archives = new List<Archive>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT * " +
        $"FROM ENGEMANINTRANET.ARCHIVE " +
        $"WHERE POST_ID = {postId}";

        var result = sq.GetDataSet(query).Tables[0].Rows;

        for (int i = 0; i < result.Count; i++)
        {
          Archive archive = new Archive();
          archive.Id = Convert.ToInt32(result[i]["id"]);
          archive.Active = Convert.ToChar(result[i]["Active"]);
          archive.ArchiveType = Convert.ToChar(result[i]["Archive_Type"]);
          archive.Name = result[i]["Name"].ToString();
          archive.Description = result[i]["Description"].ToString();
          archive.BinaryData = (byte[])result[i]["Binary_Data"];
          archive.ChangeDate = (DateTime)result[i]["ChangeDate"];
          archives.Add(archive);
        }
        return archives;
      }
    }
    public void DeleteArchiveById(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"DELETE " +
        $"FROM ARCHIVE " +
        $"WHERE ID = {id}";

        sq.ExecuteCommand(query);
      }
    }
  }
}
