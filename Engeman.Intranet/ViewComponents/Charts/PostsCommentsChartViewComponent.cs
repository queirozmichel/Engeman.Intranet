using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.Text.Json.Nodes;

namespace Engeman.Intranet.ViewComponents.Charts
{
  public class PostsCommentsChartViewComponent : ViewComponent
  {
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public PostsCommentsChartViewComponent(IPostRepository postRepository, ICommentRepository commentRepository)
    {
      _postRepository = postRepository;
      _commentRepository = commentRepository;
    }

    public IViewComponentResult Invoke(string orderBy)
    {
      ViewBag.OrderBy = orderBy;
      return View("/Views/Shared/Components/Charts/PostsCommentsChart/Default.cshtml");
    }   
  }
}