using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Engeman.Intranet.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class UserAccountController : Controller
  {

    private readonly IUserAccountRepository _userAccountRepository;

    public UserAccountController(IUserAccountRepository userAccountRepository)
    {
      _userAccountRepository = userAccountRepository;
    }
    public IActionResult Index()
    {
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(HttpContext.Session.GetString("_DomainUsername").ToString());

      return View(userAccount);
    }

    public IActionResult EditUserAccount(UserAccount userAccount, List<IFormFile> Photo)
    {
      if (!ModelState.IsValid)
      {
        return PartialView("~/Views/Error/501.cshtml");
      }
      if (Photo.Count == 0)
      {
        userAccount.Photo = _userAccountRepository.GetUserAccountByDomainUsername(userAccount.DomainAccount).Photo;
      }
      foreach (var item in Photo)
      {
        if (item.Length > 0)
        {
          using (var stream = new MemoryStream())
          {
            item.CopyTo(stream);
            userAccount.Photo = stream.ToArray();
          }
        }
      }
      _userAccountRepository.UpdateUserAccount(userAccount);

      return RedirectToAction("index", "useraccount");
    }

    public IActionResult CheckPermissions(int userIdPost, string method)
    {
      var sessionDomainAccount = HttpContext.Session.GetString("_DomainUsername").ToString();
      var userDomainAccount = _userAccountRepository.GetDomainAccountById(userIdPost);
      var userPermissions = _userAccountRepository.GetUserPermissionsByDomainUsername(sessionDomainAccount);

      if (method == "edit")
      {
        if (userPermissions.EditAnyPost == true)
        {
          return Json("EditAnyPost");
        }
        else if (userPermissions.EditOwnerPost == true)
        {
          if (userDomainAccount == sessionDomainAccount)
          {
            return Json("EditOwnerPost");
          }
          else
          {
            return Json("CannotEditAnyonePost");
          }
        }
        else if (userPermissions.EditAnyPost == false && userPermissions.EditOwnerPost == false && userDomainAccount != sessionDomainAccount)
        {
          return Json("CannotEditAnyonePost");
        }
        else if (userPermissions.EditAnyPost == false && userPermissions.EditOwnerPost == false)
        {
          return Json("CannotEditOwnerPost");
        }
      }
      else if (method == "delete")
      {
        if (userPermissions.DeleteAnyPost == true)
        {
          return Json("DeleteAnyPost");
        }
        else if (userPermissions.DeleteOwnerPost == true)
        {
          if (userDomainAccount == sessionDomainAccount)
          {
            return Json("DeleteOwnerPost");
          }
          else
          {
            return Json("CannotDeleteAnyonePost");
          }
        }
        else if (userPermissions.DeleteAnyPost == false && userPermissions.DeleteOwnerPost == false && userDomainAccount != sessionDomainAccount)
        {
          return Json("CannotDeleteAnyonePost");
        }
        else if (userPermissions.DeleteAnyPost == false && userPermissions.DeleteOwnerPost == false)
        {
          return Json("CannotDeleteOwnerPost");
        }
      }
      return Json(false);
    }
  }
}
