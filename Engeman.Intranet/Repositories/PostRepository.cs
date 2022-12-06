using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using Microsoft.Extensions.Hosting;
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
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USER_ACCOUNT_ID, " +
          "POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.DESCRIPTION as DEPARTMENT " +
          "FROM POST " +
          "LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
          "INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID " +
          "INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID " +
          "WHERE POST.ACTIVE = 1 " +
          "GROUP BY POST.ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID, POST.POST_TYPE, POST.CHANGE_DATE, " +
          "UA.NAME, UA.MODERATOR, D.ID, D.DESCRIPTION";

        var result = sq.GetDataSet(query).Tables[0];

        bool revised;
        bool restricted;
        int authorPostId;
        bool moderator;
        for (int i = 0; i < result.Rows.Count; i++)
        {
          revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          authorPostId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
          moderator = user.Moderator;

          if (revised == false && authorPostId != user.Id && moderator == false)
          {
            continue;
          }

          if (restricted == true)
          {
            query =
            $"SELECT COUNT(*)" +
            $"FROM POSTRESTRICTION " +
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
          postDto.Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
          postDto.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["Change_Date"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Department"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();
          posts.Add(postDto);
        }
      }
      return posts;
    }

    public List<Post> GetPostsByUserId(int userId)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        List<Post> posts = new List<Post>();

        var query =
        $"SELECT * " +
        $"FROM POST " +
        $"WHERE USER_ACCOUNT_ID = {userId} ";

        var result = sq.GetDataSet(query).Tables[0];
        if (result.Rows.Count == 0)
        {
          return new List<Post>();
        }
        else
        {
          for (int i = 0; i < result.Rows.Count; i++)
          {
            Post post = new Post();
            post.Id = Convert.ToInt32(result.Rows[i]["Id"]);
            post.Active = Convert.ToBoolean(result.Rows[i]["Active"]);
            post.Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
            post.Subject = result.Rows[i]["Subject"].ToString();
            post.Description = result.Rows[i]["Description"].ToString();
            post.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
            post.Keywords = result.Rows[i]["Keywords"].ToString();
            post.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
            post.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
            post.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
            post.ChangeDate = (DateTime)result.Rows[i]["Change_Date"];
            posts.Add(post);
          }
          return posts;
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
          $"POST " +
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
          $"FROM POST " +
          $"WHERE ID = {id}";

        var result = sq.GetDataSet(query).Tables[0].Rows[0];

        post.Id = Convert.ToInt32(result["Id"]);
        post.Active = Convert.ToBoolean(result["Active"]);
        post.Restricted = Convert.ToBoolean(result["Restricted"]);
        post.Subject = result["Subject"].ToString();
        post.Description = result["Description"].ToString();
        post.CleanDescription = result["Clean_Description"].ToString();
        post.Keywords = result["Keywords"].ToString();
        post.UserAccountId = Convert.ToInt32(result["User_Account_Id"]);
        post.PostType = Convert.ToChar(result["Post_Type"].ToString());
        post.Revised = Convert.ToBoolean(result["Revised"]);
        post.ChangeDate = (DateTime)result["Change_Date"];
      }
      return post;
    }

    public void AddPost(NewPostViewModel newPost)
    {
      var query = "";

      using (StaticQuery sq = new StaticQuery())
      {
        string[] paramters = { "BinaryData;byte" };

        query =
        $"INSERT INTO " +
        $"POST " +
        $"(RESTRICTED, SUBJECT, DESCRIPTION, CLEAN_DESCRIPTION, KEYWORDS, USER_ACCOUNT_ID, POST_TYPE, REVISED) OUTPUT INSERTED.ID " +
        $"VALUES ('{newPost.Restricted}', '{newPost.Subject}', N'{newPost.Description}', " +
        $"'{newPost.CleanDescription}', '{newPost.Keywords}', {newPost.UserAccountId}, " +
        $"'{newPost.PostType}', '{newPost.Revised}')";

        var outputPostId = sq.GetDataToInt(query);

        for (int i = 0; i < newPost.Files.Count; i++)
        {
          Object[] values = { newPost.Files[i].BinaryData };
          query =
          "INSERT " +
          "INTO POSTFILE(NAME, BINARY_DATA, POST_ID) " +
          $"VALUES('{newPost.Files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {outputPostId}) ";

          sq.ExecuteCommand(query, paramters, values);
        }

        if (newPost.DepartmentsList != null)
        {
          for (int i = 0; i < newPost.DepartmentsList.Count; i++)
          {
            query =
            $"INSERT INTO " +
            $"POSTRESTRICTION(POST_ID, DEPARTMENT_ID) " +
            $"VALUES({outputPostId},{newPost.DepartmentsList[i]}) ";

            sq.ExecuteCommand(query);
          }
        }

      }
    }

    public void AddPostFile(int postId, List<NewPostFileViewModel> files)
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
          "INTO POSTFILE(NAME, BINARY_DATA, POST_ID) " +
          $"VALUES('{files[i].Name}', Convert(VARBINARY(MAX),@BinaryData), {postId}) ";

          sq.ExecuteCommand(query, paramters, values);
        }
      }
    }

    public void UpdatePost(PostEditViewModel editedPost)
    {
      using (StaticQuery sq = new StaticQuery())
      {
        var update =
        $"UPDATE " +
        $"POST " +
        $"SET RESTRICTED = '{editedPost.Restricted}', SUBJECT = '{editedPost.Subject}', DESCRIPTION = N'{editedPost.Description}', " +
        $"CLEAN_DESCRIPTION = '{editedPost.Description}', KEYWORDS = '{editedPost.Keywords}', POST_TYPE = '{editedPost.PostType}', REVISED = '{editedPost.Revised}' " +
        $"WHERE ID = {editedPost.Id}";

        var delete =
        $"DELETE " +
        $"FROM POSTRESTRICTION " +
        $"WHERE POST_ID = {editedPost.Id} ";

        sq.ExecuteCommand(update);

        if (editedPost.Restricted == false)
        {
          sq.ExecuteCommand(delete);
        }
        else
        {
          sq.ExecuteCommand(delete);

          for (int i = 0; i < editedPost.DepartmentsList.Count; i++)
          {
            var insert =
            $"INSERT INTO " +
            $"POSTRESTRICTION(POST_ID, DEPARTMENT_ID) " +
            $"VALUES({editedPost.Id},{editedPost.DepartmentsList[i]}) ";
            sq.ExecuteCommand(insert);
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
        $"FROM POSTRESTRICTION " +
        $"WHERE POST_ID = {id}";

        var result = sq.GetDataSet(query).Tables[0];
        for (int i = 0; i < result.Rows.Count; i++)
        {
          departments.Add(Convert.ToInt32(result.Rows[i]["Department_Id"]));
        }
        return departments;
      }
    }

    public List<PostDto> GetPostsWithUnrevisedComments()
    {
      List<PostDto> posts = new List<PostDto>();

      string query =
      "SELECT DISTINCT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.CLEAN_DESCRIPTION, UA.ID AS USER_ACCOUNT_ID, " +
          "POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.ID as DEPARTMENT_ID, D.DESCRIPTION as DEPARTMENT " +
      "FROM POST " +
      "INNER JOIN COMMENT AS C ON C.POST_ID = POST.ID " +
      "LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
      "INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID " +
      "INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID " +
      "WHERE C.REVISED = 0 ";

      using (StaticQuery sq = new StaticQuery())
      {
        var result = sq.GetDataSet(query).Tables[0];

        for (int i = 0; i < result.Rows.Count; i++)
        {
          PostDto postDto = new PostDto();
          postDto.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postDto.Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
          postDto.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postDto.Subject = result.Rows[i]["Subject"].ToString();
          postDto.ChangeDate = result.Rows[i]["Change_Date"].ToString();
          postDto.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postDto.CleanDescription = result.Rows[i]["Clean_Description"].ToString();
          postDto.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postDto.DepartmentDescription = result.Rows[i]["Department"].ToString();
          postDto.UserAccountName = result.Rows[i]["Name"].ToString();
          postDto.HasUnrevisedComments = true;

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
      $"SET ACTIVE = '{post.Active}', RESTRICTED = '{post.Restricted}', SUBJECT = '{post.Subject}', DESCRIPTION = N'{post.Description}', CLEAN_DESCRIPTION = '{post.CleanDescription}', " +
      $"KEYWORDS = '{post.Keywords}', USER_ACCOUNT_ID = {post.UserAccountId}, " +
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
