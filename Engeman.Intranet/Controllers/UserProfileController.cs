using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Engeman.Intranet.Controllers
{
  public class UserProfileController : Controller
  {

    private readonly IUserAccountRepository _userAccountRepository;

    public UserProfileController(IUserAccountRepository userAccountRepository)
    {
      _userAccountRepository = userAccountRepository;
    }
    public IActionResult Index()
    {
      var userProfile = _userAccountRepository.GetUserProfile(HttpContext.Session.GetString("_DomainUsername").ToString());

      return View(userProfile);
    }
  }
}
