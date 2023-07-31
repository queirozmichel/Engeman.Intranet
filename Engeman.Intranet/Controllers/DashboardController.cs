using Engeman.Intranet.Extensions;
using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class DashboardController : RootController
  {
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public DashboardController(IPostRepository postRepository, ICommentRepository commentRepository)
    {
      _postRepository = postRepository;
      _commentRepository = commentRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView("Index");
    }

    public JsonResult PostsCommentsChart(string orderBy)
    {
      var posts = new List<Post>();
      var comments = new List<Comment>();
      var today = DateTime.Today;
      var data = new JsonArray();
      string month;
      int monthNumber;
      int year;
      int postsCount;
      int commentsCount;

      if (orderBy == "All")
      {
        try
        {
          posts = _postRepository.Get();
          comments = _commentRepository.Get();
        }
        catch (Exception) { }
      }
      else if (orderBy == "CurrentUser")
      {
        try
        {
          posts = _postRepository.GetByUsername(HttpContext.Session.Get<string>("_CurrentUsername"));
          comments = _commentRepository.GetByUsername(HttpContext.Session.Get<string>("_CurrentUsername"));
        }
        catch (Exception) { }
      }

      for (int i = 11; i >= 0; i--)
      {
        postsCount = 0;
        commentsCount = 0;
        monthNumber = today.AddMonths(-i).Month;
        month = today.AddMonths(-i).GetDateTimeFormats()[28].Substring(0, 3);
        year = today.AddMonths(-i).Year;
        for (int j = 0; j < posts.Count; j++)
        {
          if (posts[j].ChangeDate.Month == monthNumber && posts[j].ChangeDate.Year == year) postsCount++;
        }
        for (int j = 0; j < comments.Count; j++)
        {
          if (comments[j].ChangeDate.Month == monthNumber && comments[j].ChangeDate.Year == year) commentsCount++;
        }
        data.Add(new { label = month + "/" + year.ToString().Substring(2), posts = postsCount, comments = commentsCount });
      }

      return Json(data);
    }
  }
}