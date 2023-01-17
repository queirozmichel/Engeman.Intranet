using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class PageHeaderViewComponent : ViewComponent
  {
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;

    public PageHeaderViewComponent(ICommentRepository commentRepository, IPostRepository postRepository)
    {
      _commentRepository = commentRepository;
      _postRepository = postRepository;
    }

    public IViewComponentResult Invoke(string title)
    {
      var username = HttpContext.Session.GetString("_Username");
      var firstName = username.Substring(0, username.IndexOf(".")).ToUpper();
      var hourOfDay = DateTime.Now.Hour;
      string greetings;
      if (hourOfDay >= 5 && hourOfDay < 12)
      {
        greetings = "Bom dia, " + firstName + "!";
      }
      else if (hourOfDay >= 12 && hourOfDay < 18)
      {
        greetings = "Boa tarde, " + firstName + "!";
      }
      else
      {
        greetings = "Boa noite, " + firstName + "!";
      }

      ViewBag.Title = title;
      ViewBag.Greetings = greetings;
      ViewBag.CommentsCount = _commentRepository.CountByUsername(username);
      ViewBag.PostsCount = _postRepository.CountByUsername(username);

      return View();
    }
  }
}