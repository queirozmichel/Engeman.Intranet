using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Authorization;
using Engeman.Intranet.Models.ViewModels;
using System.Linq.Dynamic.Core;

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
    public IActionResult Grid()
    {
      bool isAjaxCall = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
      ViewBag.IsAjaxCall = isAjaxCall;
      return PartialView("UsersGrid");
    }

    public JsonResult DataGrid(string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<UserGridViewModel> users = null;
      IQueryable paginatedUsers;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      string orderedField = String.Format("{0} {1}", field, order);

      users = FilterUsers(filterHeader);
      total = users.Count();

      if (!String.IsNullOrWhiteSpace(searchPhrase))
      {
        users = FilterUsersBySearchPhrase(users, searchPhrase);
        total = users.Count();
      }

      if (rowCount == -1)
      {
        rowCount = total;
      }

      paginatedUsers = OrderedUsers(users, orderedField, current, rowCount);

      return Json(new { rows = paginatedUsers, current, rowCount, total });
    }

    public IQueryable<UserGridViewModel> FilterUsersBySearchPhrase(IQueryable<UserGridViewModel> users, string searchPhrase)
    {
      int id = 0;
      int.TryParse(searchPhrase, out id);
      users = users.Where("name.Contains(@0, StringComparison.OrdinalIgnoreCase) OR username.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR id == (@1) OR department.Contains(@0, StringComparison.OrdinalIgnoreCase)", searchPhrase, id);
      return users;
    }

    public IQueryable<UserGridViewModel> FilterUsers(string filterHeader)
    {
      var user = _userAccountRepository.GetById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      IQueryable<UserGridViewModel> users = _userAccountRepository.GetUsersGrid().AsQueryable();

      if (filterHeader == "actives")
      {
        return users = users.Where("active == (@0)", true);
      }
      else if (filterHeader == "moderators")
      {
        return users = users.Where("moderator == (@0)", true);
      }
      else if (filterHeader == "novices")
      {
        return users = users.Where("novice == (@0)", true);
      }
      else
      {
        return users;
      }
    }

    public IQueryable OrderedUsers(IQueryable<UserGridViewModel> users, string orderedField, int current, int rowCount)
    {
      IQueryable aux;
      return aux = users.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
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

    [HttpGet]
    public IActionResult NewUser()
    {
      ViewBag.Departments = _departmentRepository.Get();
      return PartialView("NewUserForm");
    }

    [HttpPost]
    public IActionResult NewUser(IFormCollection formData)
    {
      var newUser = new NewUserViewModel(formData["name"], formData["username"], Convert.ToInt32(formData["departmentId"]), Convert.ToInt32(formData["permission"]));
      var aux = _userAccountRepository.Add(newUser);
      if (aux == true)
      {
        return PartialView("UsersGrid");
      }
      else
      {
        return Json(false);
      }
    }
  }
}