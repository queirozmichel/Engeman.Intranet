using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Engeman.Intranet.ViewComponents
{
  public class PostsMenuViewComponent : ViewComponent
  {
    private readonly IUserAccountRepository _userAccount;
    private readonly IPostRepository _postRepository;

    public PostsMenuViewComponent(IUserAccountRepository userAccount, IPostRepository postRepository)
    {
      _userAccount = userAccount;
      _postRepository = postRepository;
    }

    public IViewComponentResult Invoke()
    {
      var userDomain = HttpContext.Session.GetString("_DomainUsername");
      var user = _userAccount.GetUserAccountByDomainUsername (userDomain);
      ViewBag.PostsNotRevisedCount = _postRepository.GetPostsByRestriction(user).AsQueryable().Where("revised == (@0)", false).Count();

      return View(user);
    }
  }
}
