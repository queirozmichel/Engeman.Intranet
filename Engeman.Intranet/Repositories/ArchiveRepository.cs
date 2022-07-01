using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using System;

namespace Engeman.Intranet.Repositories
{
  public class ArchiveRepository : IArchiveRepository
  {
    public Archive GetArchiveByPostId(int postId)
    {
      Archive archive = new Archive();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = 
        $"SELECT * " +
        $"FROM ENGEMANINTRANET.ARCHIVE " +
        $"WHERE POST_ID = {postId}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        archive.Id = Convert.ToInt32(result["id"]);
        archive.Active = Convert.ToChar(result["Active"]);
        archive.ArchiveType = Convert.ToChar(result["Archive_Type"]);
        archive.Name = result["Name"].ToString();
        archive.Description = result["Description"].ToString();
        archive.BinaryData = (byte[])result["Binary_Data"];
        archive.ChangeDate = (DateTime)result["ChangeDate"];

        return archive;
      }      
    }
  }
}
