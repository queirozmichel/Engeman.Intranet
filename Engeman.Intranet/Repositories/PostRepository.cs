using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
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
          postDto.Department.Description = result.Rows[i]["Description"].ToString();
          postDto.UserAccount.Name = result.Rows[i]["Name"].ToString();

          posts.Add(postDto);
        }
      }
      return posts;
    }
  }
}
