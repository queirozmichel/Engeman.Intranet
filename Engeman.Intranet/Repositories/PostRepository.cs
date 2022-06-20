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
          "POST.ID, POST.SUBJECT, POST.CLEAN_DESCRIPTION, POST.CHANGEDATE, UA.NAME, D.DESCRIPTION " +
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
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.DepartmentDescription = result.Rows[i]["Description"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();

          posts.Add(postDto);
        }
      }
      return posts;
    }

    public void InsertQuestion(AskQuestionDto askQuestionDto)
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
  }
}
