using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public class PostRepository : IPostRepository
  {
    public List<PostDto> GetPostsByRestriction(UserAccount user)
    {
      List<PostDto> posts = new List<PostDto>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USERACCOUNT_ID, " +
          "POST.POST_TYPE, POST.CHANGEDATE, UA.NAME, UA.MODERATOR, D.ID as DEPARTMENT_ID, D.DESCRIPTION as DEPARTMENT, PF.FILE_TYPE " +
          "FROM POST " +
          "LEFT JOIN POST_FILE AS PF ON PF.POST_ID = POST.ID " +
          "INNER JOIN USERACCOUNT AS UA ON POST.USERACCOUNT_ID = UA.ID " +
          "INNER JOIN DEPARTMENT AS D ON POST.DEPARTMENT_ID = D.ID " +
          "WHERE POST.ACTIVE = 'S' " +
          "GROUP BY POST.ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID, POST.POST_TYPE, POST.CHANGEDATE, PF.FILE_TYPE, " +
          "UA.NAME, UA.MODERATOR, D.ID, D.DESCRIPTION";

        var result = sq.GetDataSet(query).Tables[0];

        bool revised;
        char restricted;
        int authorPostId;
        bool moderator;
        for (int i = 0; i < result.Rows.Count; i++)
        {
          revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          authorPostId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          restricted = Convert.ToChar(result.Rows[i]["Restricted"]);
          moderator = user.Moderator;

          if (revised == false && authorPostId != user.Id && moderator == false)
          {
            continue;
          }

          if (restricted == 'S')
          {
            query =
            $"SELECT COUNT(*)" +
            $"FROM POST_DEPARTMENT " +
            $"WHERE POST_ID = {result.Rows[i]["Post_Id"]} AND DEPARTMENT_ID = {user.DepartmentId}";

            var aux = sq.GetDataToInt(query);
            //se não fizer parte do setor que há na tabela de restrição e se não for o autor da postagem
            if (aux == 0 && authorPostId != user.Id && moderator == false)
            {
              continue;
            }
          }
          PostDto postDto = new PostDto();
          postDto.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postDto.Restricted = Convert.ToChar(result.Rows[i]["Restricted"]);
          postDto.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["ChangeDate"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Department"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();
          postDto.FileType = result.Rows[i]["File_Type"].ToString();

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
        $"(ACTIVE, RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_TYPE, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES ('{askQuestionDto.Active}', '{askQuestionDto.Restricted}', '{askQuestionDto.Subject}', '{askQuestionDto.Description}', " +
        $"'{askQuestionDto.CleanDescription}', '{askQuestionDto.Keywords}', {askQuestionDto.UserAccountId}, {askQuestionDto.DepartmentId}, " +
        $"'{askQuestionDto.PostType}', '{askQuestionDto.Revised}')";

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
        post.PostType = Convert.ToChar(result["Post_Type"].ToString());
        post.Revised = Convert.ToBoolean(result["Revised"]);
        post.ChangeDate = (DateTime)result["ChangeDate"];
      }
      return post;
    }

    public void AddPostFile(AskQuestionDto askQuestionDto, List<PostFile> files)
    {
      var query = "";

      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        query =
        $"INSERT INTO " +
        $"ENGEMANINTRANET.POST " +
        $"(RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USERACCOUNT_ID, DEPARTMENT_ID, POST_TYPE, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES ('{askQuestionDto.Restricted}', '{askQuestionDto.Subject}', '{askQuestionDto.Description}', " +
        $"'{askQuestionDto.CleanDescription}', '{askQuestionDto.Keywords}', {askQuestionDto.UserAccountId}, {askQuestionDto.DepartmentId}, " +
        $"'{askQuestionDto.PostType}', '{askQuestionDto.Revised}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < files.Count; i++)
        {
          Object[] values = { files[i].BinaryData };
          query =
          "INSERT " +
          "INTO POST_FILE(FILE_TYPE, NAME, DESCRIPTION, BINARY_DATA, POST_ID) " +
          $"VALUES('{files[i].FileType}', '{files[i].Name}', '{files[i].Description}', Convert(VARBINARY(MAX),@BinaryData), {outputPostId}) ";

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

    public void AddPostFile(int id, List<PostFile> files)
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
          "INTO POST_FILE(FILE_TYPE, NAME, DESCRIPTION, BINARY_DATA, POST_ID) " +
          $"VALUES('{files[i].FileType}', '{files[i].Name}', '{files[i].Description}', Convert(VARBINARY(MAX),@BinaryData), {id}) ";

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
        $"CLEAN_DESCRIPTION = '{askQuestionDto.Description}', KEYWORDS = '{askQuestionDto.Keywords}', REVISED = '{askQuestionDto.Revised}' " +
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

    public void UpdatePostFile(int id, AskQuestionDto postInformation, List<PostFile> files)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        string update;
        string delete;

        UpdateQuestion(id, postInformation);

        for (int i = 0; i < files.Count; i++)
        {
          update =
          $"UPDATE " +
          $"ENGEMANINTRANET.POST_FILE " +
          $"SET FILE_TYPE = '{files[i].FileType}', DESCRIPTION = '{postInformation.Description}' " +
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

    public List<PostDto> GetPostsWithUnrevisedComments()
    {
      List<PostDto> posts = new List<PostDto>();

      string query =
      "SELECT DISTINCT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USERACCOUNT_ID, " +
          "POST.POST_TYPE, POST.CHANGEDATE, UA.NAME, UA.MODERATOR, D.ID as DEPARTMENT_ID, D.DESCRIPTION as DEPARTMENT, PF.FILE_TYPE " +
      "FROM POST " +
      "INNER JOIN POST_COMMENT AS PC ON PC.POST_ID = POST.ID " +
      "LEFT JOIN POST_FILE AS PF ON PF.POST_ID = POST.ID " +
      "INNER JOIN USERACCOUNT AS UA ON POST.USERACCOUNT_ID = UA.ID " +
      "INNER JOIN DEPARTMENT AS D ON POST.DEPARTMENT_ID = D.ID " +
      "WHERE PC.REVISED = 0 ";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          PostDto postDto = new PostDto();
          postDto.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postDto.Restricted = Convert.ToChar(result.Rows[i]["Restricted"]);
          postDto.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["ChangeDate"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["UserAccount_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Department"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();
          postDto.FileType = result.Rows[i]["File_Type"].ToString();

          posts.Add(postDto);
        }
        return posts;
      }
    }

    public bool UpdatePost(int id, Post post)
    {
      string query =
      $"UPDATE " +
      $"POST " +
      $"SET ACTIVE = '{post.Active}', RESTRICTED = '{post.Restricted}', SUBJECT = '{post.Subject}', DESCRIPTION = '{post.Description}', CLEAN_DESCRIPTION = '{post.CleanDescription}', " +
      $"KEYWORDS = '{post.Keywords}', USERACCOUNT_ID = {post.UserAccountId}, DEPARTMENT_ID = {post.DepartmentId}, " +
      $"POST_TYPE = '{post.PostType}', REVISED = '{post.Revised}' " +
      $"WHERE ID = '{id}'";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = Convert.ToBoolean(sq.ExecuteCommand(query));

        return result;
      }
    }
  }
}
