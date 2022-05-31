using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Engeman.Intranet.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class UserProfileController : Controller  {

    private readonly IUserAccountRepository _userAccountRepository;

    public UserProfileController(IUserAccountRepository userAccountRepository)
    {
      _userAccountRepository = userAccountRepository;
    }
    public IActionResult Index()
    {
      var profileInfos = _userAccountRepository.GetUserProfile(HttpContext.Session.GetString("_DomainUsername").ToString());

      return View(profileInfos);
    }

    public IActionResult EditUserProfile(UserProfile userProfile, List<IFormFile> Photo)
    {
      if (Photo.Count == 0)
      {
       userProfile.Photo =  _userAccountRepository.GetUserProfile(userProfile.DomainAccount).Photo;
      }
      foreach (var item in Photo)
      {
        if (item.Length>0)
        {
          using (var stream = new MemoryStream())
          {
            item.CopyTo(stream);
            userProfile.Photo = stream.ToArray();
          }
        }
      }
      _userAccountRepository.UpdateUserProfile(userProfile);

      return RedirectToAction("index","userprofile");
    }
  }
}
