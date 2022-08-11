using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostRepository : IPostRepository
  {
    public List<PostDto> GetPostsByRestriction(int userDepartmentId, int userIdSession)
    {
      List<PostDto> posts = new List<PostDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USERACCOUNT_ID, POST.CHANGEDATE, POST.POST_TYPE, UA.NAME, D.ID as DEPARTMENT_ID, D.DESCRIPTION as DEPARTMENT, A.ARCHIVE_TYPE " +
          "FROM POST " +
          "LEFT JOIN ARCHIVE AS A ON A.POST_ID = POST.ID " +
          "INNER JOIN USERACCOUNT AS UA ON POST.USERACCOUNT_ID = UA.ID " +
          "INNER JOIN DEPARTMENT AS D ON POST.DEPARTMENT_ID = D.ID " +
          "WHERE POST.ACTIVE = 'S'";

        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {

          if (Convert.ToChar(result.Rows[i]["Restricted"]) == 'S')
          {
            query =
            $"SELECT COUNT(*)" +
            $"FROM POST_DEPARTMENT " +
            $"WHERE POST_ID = {result.Rows[i]["Post_Id"]} AND DEPARTMENT_ID = {userDepartmentId}";

            var aux = sq.GetDataToInt(query);
            //se não houver registro na tabela de restrição e se noa for o autor da postagem
            if (aux == 0 && Convert.ToInt32(result.Rows[i]["UserAccount_Id"]) != userIdSession)
            {
              continue;
            }
          }
          PostDto postDto = new PostDto();
          postDto.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postDto.Restricted = Convert.ToChar(result.Rows[i]["Restricted"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["ChangeDate"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Department"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();
          postDto.ArchiveType = result.Rows[i]["Archive_Type"].ToString();

          posts.Add(postDto);
        }
      }
      return posts;
    }

    public void AddQuestion(AskQuestionDto askQuestionDto)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"INSERT INTO " +
        $"ENGEMANINTRANET.POST " +
        $"(ACTIVE, RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_TYPE) OUTPUT INSERTED.ID " +
        $"VALUES ('{askQuestionDto.Active}', '{askQuestionDto.Restricted}', '{askQuestionDto.Subject}', '{askQuestionDto.Description}', " +
        $"'{askQuestionDto.CleanDescription}', '{askQuestionDto.Keywords}', {askQuestionDto.UserAccountId}, {askQuestionDto.DepartmentId}, " +
        $"'{askQuestionDto.PostType}')";

        var outputPostId = sq.GetDataToInt(query);

        if (askQuestionDto.DepartmentsList != null)
        {
          for (int i = 0; i < askQuestionDto.DepartmentsList.Count; i++)
          {
            query =
            $"INSERT INTO " +
            $"POST_DEPARTMENT(POST_ID, DEPARTMENT_ID) " +
            $"VALUES({outputPostId},{askQuestionDto.DepartmentsList[i]}) ";

            sq.ExecuteCommand(query);
          }
        }
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

        if (askQuestionDto.DepartmentsList != null)
        {
          for (int i = 0; i < askQuestionDto.DepartmentsList.Count; i++)
          {
            query =
            $"INSERT INTO " +
            $"POST_DEPARTMENT(POST_ID, DEPARTMENT_ID) " +
            $"VALUES({outputPostId},{askQuestionDto.DepartmentsList[i]}) ";

            sq.ExecuteCommand(query);
          }
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
        var update =
        $"UPDATE " +
        $"ENGEMANINTRANET.POST " +
        $"SET RESTRICTED = '{askQuestionDto.Restricted}', SUBJECT = '{askQuestionDto.Subject}', DESCRIPTION = '{askQuestionDto.Description}', " +
        $"CLEAN_DESCRIPTION = '{askQuestionDto.Description}', KEYWORDS = '{askQuestionDto.Keywords}' " +
        $"WHERE ID = {id}";

        var delete =
        $"DELETE " +
        $"FROM POST_DEPARTMENT " +
        $"WHERE POST_ID = {id} ";

        sq.ExecuteCommand(update);

        if (askQuestionDto.Restricted == 'N')
        {
          sq.ExecuteCommand(delete);
        }
        else
        {
          sq.ExecuteCommand(delete);

          for (int i = 0; i < askQuestionDto.DepartmentsList.Count; i++)
          {
            var insert =
            $"INSERT INTO " +
            $"POST_DEPARTMENT(POST_ID, DEPARTMENT_ID) " +
            $"VALUES({id},{askQuestionDto.DepartmentsList[i]}) ";
            sq.ExecuteCommand(insert);
          }
        }
      }
    }

    public void UpdateArchivePost(int id, AskQuestionDto postInformation, List<Archive> archives)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string update;
        string delete;

        UpdateQuestion(id, postInformation);

        for (int i = 0; i < archives.Count; i++)
        {
          update =
          $"UPDATE " +
          $"ENGEMANINTRANET.ARCHIVE " +
          $"SET ARCHIVE_TYPE = '{archives[i].ArchiveType}', DESCRIPTION = '{postInformation.Description}' " +
          $"WHERE POST_ID = {id}";

          sq.ExecuteCommand(update);

          if (postInformation.Restricted == 'N')
          {
            delete =
            $"DELETE " +
            $"FROM POST_DEPARTMENT " +
            $"WHERE POST_ID = {id} ";

            sq.ExecuteCommand(delete);
          }
        }

        //var update =
        //$"UPDATE " +
        //$"ENGEMANINTRANET.POST " +
        //$"SET RESTRICTED = '{postInformation.Restricted}', SUBJECT = '{postInformation.Subject}', DESCRIPTION = '{postInformation.Description}', " +
        //$"CLEAN_DESCRIPTION = '{postInformation.Description}', KEYWORDS = '{postInformation.Keywords}' " +
        //$"WHERE ID = {id}";

        //var delete =
        //$"DELETE " +
        //$"FROM POST_DEPARTMENT " +
        //$"WHERE POST_ID = {id} ";

        //sq.ExecuteCommand(update);

        //for (int i = 0; i < archives.Count; i++)
        //{
        //  update =
        //  $"UPDATE " +
        //  $"ENGEMANINTRANET.ARCHIVE " +
        //  $"SET ARCHIVE_TYPE = '{archives[i].ArchiveType}', DESCRIPTION = '{postInformation.Description}' " +
        //  $"WHERE POST_ID = {id}";

        //  sq.ExecuteCommand(update);

        //  if (postInformation.Restricted == 'N')
        //  {
        //    sq.ExecuteCommand(delete);
        //  }
        //  else
        //  {
        //    sq.ExecuteCommand(delete);

        //    for (i = 0; i < postInformation.DepartmentsList.Count; i++)
        //    {
        //      var insert =
        //      $"INSERT INTO " +
        //      $"POST_DEPARTMENT(POST_ID, DEPARTMENT_ID) " +
        //      $"VALUES({id},{postInformation.DepartmentsList[i]}) ";
        //      sq.ExecuteCommand(insert);
        //    }
        //  }
        //}
      }
    }

    public List<int> GetRestrictedDepartmentsIdByPost(int id)
    {
      List<int> departments = new List<int>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT DEPARTMENT_ID " +
        $"FROM POST_DEPARTMENT " +
        $"WHERE POST_ID = {id}";

        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          departments.Add(Convert.ToInt32(result.Rows[i]["Department_Id"]));
        }
        return departments;
      }
    }

    public int GetPostsCountByUserId(int id)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var query =
        $"SELECT COUNT(*) " +
        $"FROM POST " +
        $"WHERE USERACCOUNT_ID = {id}";

        var result = sq.GetDataToInt(query);

        return result;
      }
    }
  }
}
