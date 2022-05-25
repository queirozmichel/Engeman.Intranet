using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Engeman.Intranet.Models;

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

    public IActionResult EditUserProfile(UserProfile userProfile)
    {
      _userAccountRepository.UpdateUserProfile(userProfile);

      return RedirectToAction("index","userprofile");
    }
  }
}
