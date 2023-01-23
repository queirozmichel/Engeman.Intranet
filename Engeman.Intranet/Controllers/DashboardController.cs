﻿using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class DashboardController : Controller
  {
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public DashboardController(IPostRepository postRepository, ICommentRepository commentRepository)
    {
      _postRepository = postRepository;
      _commentRepository = commentRepository;
    }

    public IActionResult Index()
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      ViewBag.IsAjaxCall = isAjaxCall;

      return PartialView("Index");
    }

    public JsonResult PostsCommentsChart(string orderBy)
    {
      List<Post> posts = new List<Post>();
      List<Comment> comments = new List<Comment>();
      if (orderBy == "All")
      {
        posts = _postRepository.Get();
        comments = _commentRepository.Get();

      }
      else if(orderBy == "CurrentUser")
      {
        posts = _postRepository.GetByUsername(HttpContext.Session.GetString("_Username"));
        comments = _commentRepository.GetByUsername(HttpContext.Session.GetString("_Username"));
      }

      var today = DateTime.Now;
      var data = new JsonArray();
      string month;
      int monthNumber = 0;
      int year = 0;
      int postsCount = 0;
      int commentsCount = 0;

      for (int i = 11; i >= 0; i--)
      {
        postsCount = 0;
        commentsCount = 0;
        monthNumber = today.AddMonths(-i).Month;
        month = today.AddMonths(-i).GetDateTimeFormats()[28].Substring(0, 3);
        year = today.AddMonths(-i).Year;

        for (int j = 0; j < posts.Count; j++)
        {
          if (posts[j].ChangeDate.Month == monthNumber && posts[j].ChangeDate.Year == year)
          {
            postsCount++;
          }
        }

        for (int j = 0; j < comments.Count; j++)
        {
          if (comments[j].ChangeDate.Month == monthNumber && comments[j].ChangeDate.Year == year)
          {
            commentsCount++;
          }
        }
        data.Add(new { label = month + "/" + year.ToString().Substring(2), posts = postsCount, comments = commentsCount });
      }
      return Json(data);
    }
  }
}