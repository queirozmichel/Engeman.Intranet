using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class StatBoxesViewComponent : ViewComponent
  {
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public StatBoxesViewComponent(IPostRepository postRepository, ICommentRepository commentRepository)
    {
      _postRepository = postRepository;
      _commentRepository = commentRepository;
    }

    public IViewComponentResult Invoke()
    {
      ViewBag.Manuals = _postRepository.CountByPostType('M');
      ViewBag.Documents = _postRepository.CountByPostType('D');
      ViewBag.Questions = _postRepository.CountByPostType('Q');
      ViewBag.Informatives = _postRepository.CountByPostType('I');
      ViewBag.Comments = _commentRepository.Count();

      return View();
    }
  }
}
