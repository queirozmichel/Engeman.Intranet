using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Authorization;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class UserAccountController : Controller
  {
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public UserAccountController(IUserAccountRepository userAccountRepository, IDepartmentRepository departmentRepository)
    {
      _userAccountRepository = userAccountRepository;
      _departmentRepository = departmentRepository;
    }

    [HttpGet]
    public IActionResult UserProfile()
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      var userAccount = _userAccountRepository.GetByUsername(HttpContext.Session.GetString("_Username").ToString());
      ViewBag.Department = _departmentRepository.GetById(userAccount.DepartmentId);
      ViewBag.IsAjaxCall = isAjaxCall;

      return PartialView(userAccount);
    }

    [HttpPost]
    public IActionResult UpdateUserProfile(UserAccount userAccount, List<IFormFile> Photo)
    {
      if (!ModelState.IsValid)
      {
        return PartialView("~/Views/Error/501.cshtml");
      }
      if (Photo.Count == 0)
      {
        userAccount.Photo = _userAccountRepository.GetByUsername(userAccount.Username).Photo;
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
      _userAccountRepository.Update(userAccount);

      return Ok(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult CheckPermissions(int authorId, string method)
    {
      var sessionUsername = HttpContext.Session.GetString("_Username").ToString();
      var username = _userAccountRepository.GetUsernameById(authorId);
      var userPermissions = _userAccountRepository.GetUserPermissionsByUsername(sessionUsername);

      if (method == "edit")
      {
        if (userPermissions.EditAnyPost == true)
        {
          return Json("EditAnyPost");
        }
        else if (userPermissions.EditOwnerPost == true)
        {
          if (username == sessionUsername)
          {
            return Json("EditOwnerPost");
          }
          else
          {
            return Json("CannotEditAnyonePost");
          }
        }
        else if (userPermissions.EditAnyPost == false && userPermissions.EditOwnerPost == false && username != sessionUsername)
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
          if (username == sessionUsername)
          {
            return Json("DeleteOwnerPost");
          }
          else
          {
            return Json("CannotDeleteAnyonePost");
          }
        }
        else if (userPermissions.DeleteAnyPost == false && userPermissions.DeleteOwnerPost == false && username != sessionUsername)
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