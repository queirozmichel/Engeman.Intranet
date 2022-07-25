using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostRepository : IPostRepository
  {
    public List<PostDto> GetAllPosts()
    {
      List<PostDto> posts = new List<PostDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT " +
          "POST.ID, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USERACCOUNT_ID, POST.CHANGEDATE, POST.POST_TYPE, UA.NAME, D.DESCRIPTION " +
          "FROM POST INNER JOIN USERACCOUNT AS UA ON POST.USERACCOUNT_ID = UA.ID " +
          "INNER JOIN DEPARTMENT AS D ON POST.DEPARTMENT_ID = D.ID " +
          "WHERE POST.ACTIVE = 'S'";

        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          PostDto postDto = new PostDto();
          postDto.Id = Convert.ToInt32(result.Rows[i]["Id"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["ChangeDate"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Description"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();

          posts.Add(postDto);
        }
      }
      return posts;
    }

    public void AddQuestion(AskQuestionDto askQuestionDto)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { };
        Object[] values = { askQuestionDto };

        var query =
        $"INSERT INTO " +
        $"ENGEMANINTRANET.POST " +
        $"(ACTIVE, RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_TYPE) " +
        $"VALUES ('{askQuestionDto.Active}', '{askQuestionDto.Restricted}', '{askQuestionDto.Subject}', '{askQuestionDto.Description}', " +
        $"'{askQuestionDto.CleanDescription}', '{askQuestionDto.Keywords}', {askQuestionDto.UserAccountId}, {askQuestionDto.DepartmentId}, " +
        $"'{askQuestionDto.PostType}')";

        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public void DeletePost(int postId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { };
        Object[] values = { postId };

        var query =
          $"DELETE FROM " +
          $"ENGEMANINTRANET.POST " +
          $"WHERE ID = {postId}";

        sq.ExecuteCommand(query, paramters, values);
      }
    }

    public Post GetPostById(int id)
    {
      Post post = new Post();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
          $"SELECT " +
          $"* " +
          $"FROM ENGEMANINTRANET.POST " +
          $"WHERE ID = {id}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        post.Id = Convert.ToInt32(result["id"]);
        post.Active = Convert.ToChar(result["Active"]);
        post.Restricted = Convert.ToChar(result["Restricted"]);
        post.Subject = result["Subject"].ToString();
        post.Description = result["Description"].ToString();
        post.CleanDescription = result["Clean_Description"].ToString();
        post.Keywords = result["Keywords"].ToString();
        post.UserAccountId = Convert.ToInt32(result["UserAccount_Id"]);
        post.DepartmentId = Convert.ToInt32(result["Department_Id"]);
        post.ChangeDate = (DateTime)result["ChangeDate"];
        post.PostType = Convert.ToChar(result["Post_Type"].ToString());
      }
      return post;
    }

    public void AddArchive(AskQuestionDto askQuestionDto, List<Archive> archives)
    {
      var query = "";

      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        query =
        $"INSERT INTO " +
        $"ENGEMANINTRANET.POST " +
        $"(ACTIVE, RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_TYPE) OUTPUT INSERTED.ID " +
        $"VALUES ('{askQuestionDto.Active}', '{askQuestionDto.Restricted}', '{askQuestionDto.Subject}', '{askQuestionDto.Description}', " +
        $"'{askQuestionDto.CleanDescription}', '{askQuestionDto.Keywords}', {askQuestionDto.UserAccountId}, {askQuestionDto.DepartmentId}, " +
        $"'{askQuestionDto.PostType}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < archives.Count; i++)
        {
          Object[] values = { archives[i].BinaryData };
          query =
          "INSERT " +
          "INTO ARCHIVE(ACTIVE, ARCHIVE_TYPE, NAME, DESCRIPTION, BINARY_DATA, POST_ID) " +
          $"VALUES('{archives[i].Active}', '{archives[i].ArchiveType}', '{archives[i].Name}', '{archives[i].Description}', Convert(VARBINARY(MAX),@BinaryData), {outputPostId}) ";

          sq.ExecuteCommand(query, paramters, values);
        }

      }
    }

    public void AddArchive(int id, List<Archive> archives)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query = "";
        string[] paramters = { "BinaryData;byte" };

        for (int i = 0; i < archives.Count; i++)
        {
          Object[] values = { archives[i].BinaryData };

          query =
          "INSERT " +
          "INTO ARCHIVE(ACTIVE, ARCHIVE_TYPE, NAME, DESCRIPTION, BINARY_DATA, POST_ID) " +
          $"VALUES('{archives[i].Active}', '{archives[i].ArchiveType}', '{archives[i].Name}', '{archives[i].Description}', Convert(VARBINARY(MAX),@BinaryData), {id}) ";
          
          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public void UpdateQuestion(int id, AskQuestionDto askQuestionDto)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"UPDATE " +
        $"ENGEMANINTRANET.POST " +
        $"SET RESTRICTED = '{askQuestionDto.Restricted}', SUBJECT = '{askQuestionDto.Subject}', DESCRIPTION = '{askQuestionDto.Description}', " +
        $"CLEAN_DESCRIPTION = '{askQuestionDto.Description}', KEYWORDS = '{askQuestionDto.Keywords}' " +
        $"WHERE ID = {id}";

        sq.ExecuteCommand(query);
      }
    }

    public void UpdateArchivePost(int id, AskQuestionDto postInformation, List<Archive> archives)
    {
      //using (StaticQuery sq = new StaticQuery())
      //{
      //  UpdateQuestion(id, askQuestionDto);

      //  string[] paramters = { "BinaryData;byte" };
      //  Object[] values = { archive.BinaryData };

      //  var query =
      //  $"UPDATE " +
      //  $"ENGEMANINTRANET.ARCHIVE " +
      //  $"SET ARCHIVE_TYPE = '{archive.ArchiveType}', NAME = '{archive.Name}', DESCRIPTION = '{askQuestionDto.Description}', " +
      //  $"BINARY_DATA = Convert(VARBINARY(MAX),@BinaryData) " +
      //  $"WHERE POST_ID = {id}";

      //  sq.ExecuteCommand(query, paramters, values);
      //}

      using (StaticQuery sq = new StaticQuery())
      {
        UpdateQuestion(id, postInformation);

        for (int i = 0; i < archives.Count; i++)
        {

          var query =
          $"UPDATE " +
          $"ENGEMANINTRANET.ARCHIVE " +
          $"SET ARCHIVE_TYPE = '{archives[i].ArchiveType}', DESCRIPTION = '{postInformation.Description}' " +
          $"WHERE POST_ID = {id}";

          sq.ExecuteCommand(query);
        }
      }
    }
  }
}
