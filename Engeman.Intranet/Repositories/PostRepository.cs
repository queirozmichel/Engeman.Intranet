using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
    public class PostRepository : IPostRepository
  {
    public List<PostGridViewModel> GetByRestriction(UserAccount user)
    {
      List<PostGridViewModel> posts = new List<PostGridViewModel>();

      using (StaticQuery sq = new StaticQuery())
      {
        var query = "SELECT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.DESCRIPTION, UA.ID AS USER_ACCOUNT_ID, " +
          "POST.POST_TYPE, POST.CHANGE_DATE, UA.NAME, UA.MODERATOR, D.DESCRIPTION as DEPARTMENT " +
          "FROM POST " +
          "LEFT JOIN POSTFILE AS PF ON PF.POST_ID = POST.ID " +
          "INNER JOIN USERACCOUNT AS UA ON POST.USER_ACCOUNT_ID = UA.ID " +
          "INNER JOIN DEPARTMENT AS D ON UA.DEPARTMENT_ID = D.ID " +
          "WHERE POST.ACTIVE = 1 " +
          "GROUP BY POST.ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.DESCRIPTION, UA.ID, POST.POST_TYPE, POST.CHANGE_DATE, " +
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
          PostGridViewModel postGrid = new PostGridViewModel();
          postGrid.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postGrid.Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
          postGrid.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postGrid.Subject = result.Rows[i]["Subject"].ToString();
          postGrid.ChangeDate = result.Rows[i]["Change_Date"].ToString();
          postGrid.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postGrid.Description = result.Rows[i]["Description"].ToString();
          postGrid.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postGrid.Department = result.Rows[i]["Department"].ToString();
          postGrid.UserAccountName = result.Rows[i]["Name"].ToString();
          posts.Add(postGrid);
        }
      }
      return posts;
    }

    public List<Post> GetByUserAccountId(int userId)
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

    public void Delete(int postId)
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

    public Post Get(int postId)
    {
      Post post = new Post();
      var query = $"SELECT * FROM POST WHERE ID = {postId}";

      using (StaticQuery sq = new StaticQuery())
      {
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

    public void Add(NewPostViewModel newPost)
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

    public void Update(PostEditViewModel editedPost)
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

    public List<PostGridViewModel> GetWithUnrevisedComments()
    {
      List<PostGridViewModel> posts = new List<PostGridViewModel>();

      string query =
      "SELECT DISTINCT " +
          "POST.ID as POST_ID, POST.RESTRICTED, POST.REVISED, POST.SUBJECT, POST.DESCRIPTION, UA.ID AS USER_ACCOUNT_ID, " +
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
          PostGridViewModel postGrid = new PostGridViewModel();
          postGrid.Id = Convert.ToInt32(result.Rows[i]["Post_Id"]);
          postGrid.Restricted = Convert.ToBoolean(result.Rows[i]["Restricted"]);
          postGrid.Revised = Convert.ToBoolean(result.Rows[i]["Revised"]);
          postGrid.Subject = result.Rows[i]["Subject"].ToString();
          postGrid.ChangeDate = result.Rows[i]["Change_Date"].ToString();
          postGrid.PostType = Convert.ToChar(result.Rows[i]["Post_Type"]);
          postGrid.Description = result.Rows[i]["Description"].ToString();
          postGrid.UserAccountId = Convert.ToInt32(result.Rows[i]["User_Account_Id"]);
          postGrid.Department = result.Rows[i]["Department"].ToString();
          postGrid.UserAccountName = result.Rows[i]["Name"].ToString();
          postGrid.UnrevisedComments = true;

          posts.Add(postGrid);
        }
        return posts;
      }
    }

    public bool Update(int id, Post post)
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
