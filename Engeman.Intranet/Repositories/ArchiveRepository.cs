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
      //Archive archive = new Archive();
      List<Archive> archives = new List<Archive>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT * " +
        $"FROM ENGEMANINTRANET.ARCHIVE " +
        $"WHERE POST_ID = {postId}";

        //int numberOfArchives = Convert.ToInt32(sq.GetDataSet(query).Tables[0].Rows[0].ItemArray[0]);
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


        //var query = 
        //$"SELECT * " +
        //$"FROM ENGEMANINTRANET.ARCHIVE " +
        //$"WHERE POST_ID = {postId}";

        //var result = sq.GetDataSet(query).Tables[0].Rows[0];

        //archive.Id = Convert.ToInt32(result["id"]);
        //archive.Active = Convert.ToChar(result["Active"]);
        //archive.ArchiveType = Convert.ToChar(result["Archive_Type"]);
        //archive.Name = result["Name"].ToString();
        //archive.Description = result["Description"].ToString();
        //archive.BinaryData = (byte[])result["Binary_Data"];
        //archive.ChangeDate = (DateTime)result["ChangeDate"];

        return archives;
      }      
    }
  }
}
