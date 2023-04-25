using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Authorization;
using Engeman.Intranet.Models.ViewModels;
using System.Linq.Dynamic.Core;
using Engeman.Intranet.Extensions;
using System.Data.SqlClient;

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
      var isModerator = HttpContext.Session.Get<bool>("_IsModerator");

      if (isModerator == false) return Redirect(Request.Host.ToString());

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView("UsersGrid");
    }

    public JsonResult DataGrid(string filterHeader, int rowCount, string searchPhrase, int current)
    {
      IQueryable<UserGridViewModel> users;
      IQueryable paginatedUsers;
      int total = 0;
      var key = Request.Form.Keys.Where(k => k.StartsWith("sort")).FirstOrDefault();
      var requestKeys = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
      var order = requestKeys[key];
      var field = key.Replace("sort[", "").Replace("]", "");
      var orderedField = string.Format("{0} {1}", field, order);

      users = FilterUsers(filterHeader);

      total = users.Count();

      if (!string.IsNullOrWhiteSpace(searchPhrase))
      {
        users = FilterUsersBySearchPhrase(users, searchPhrase);
        total = users.Count();
      }

      if (rowCount == -1) rowCount = total;

      paginatedUsers = OrderedUsers(users, orderedField, current, rowCount);

      return Json(new { rows = paginatedUsers, current, rowCount, total });
    }

    public IQueryable<UserGridViewModel> FilterUsersBySearchPhrase(IQueryable<UserGridViewModel> users, string searchPhrase)
    {
      int.TryParse(searchPhrase, out int id);

      return users.Where("name.Contains(@0, StringComparison.OrdinalIgnoreCase) OR username.Contains(@0, StringComparison.OrdinalIgnoreCase) " +
        "OR id == (@1) OR department.Contains(@0, StringComparison.OrdinalIgnoreCase)", searchPhrase, id);
    }

    public IQueryable<UserGridViewModel> FilterUsers(string filterHeader)
    {
      IQueryable<UserGridViewModel> users = null;

      try { users = _userAccountRepository.GetUsersGrid().AsQueryable(); } catch (Exception) { }

      if (filterHeader == "actives") return users.Where("active == (@0)", true);
      else if (filterHeader == "moderators") return users.Where("moderator == (@0)", true);
      else if (filterHeader == "novices") return users.Where("novice == (@0)", true);
      else return users;
    }

    public IQueryable OrderedUsers(IQueryable<UserGridViewModel> users, string orderedField, int current, int rowCount)
    {
      return users.OrderBy(orderedField).Skip((current - 1) * rowCount).Take(rowCount);
    }

    [HttpGet]
    public IActionResult NewUser()
    {
      try { ViewBag.Departments = _departmentRepository.Get(); } catch (Exception) { }

      return PartialView("NewUserForm");
    }

    [HttpPost]
    public JsonResult NewUser(IFormCollection formData)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;
      var newUser = new NewUserViewModel(formData["name"], formData["username"], Convert.ToInt32(formData["departmentId"]), Convert.ToInt32(formData["permission"]));

      try { _userAccountRepository.Add(newUser, sessionUsername); }
      catch (SqlException ex)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        if (ex.Number == 2627)
        {
          messageAux = "Já existe um cadastro com o mesmo 'Nome de Usuário.";
        }
        else
        {
          messageAux = ex.Message;
        }       
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpGet]
    public IActionResult EditUserProfile()
    {
      var userAccount = new UserAccount();

      try
      {
        userAccount = _userAccountRepository.GetByUsername(HttpContext.Session.Get<string>("_CurrentUsername"));
        ViewBag.Department = _departmentRepository.GetById(userAccount.DepartmentId);
      }
      catch (Exception) { }

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView(userAccount);
    }

    [HttpPost]
    public IActionResult UpdateUserProfile(UserAccount userAccount, List<IFormFile> Photo)
    {
      if (Photo.Count == 0) try { userAccount.Photo = _userAccountRepository.GetByUsername(userAccount.Username).Photo; } catch (Exception) { }

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

      try { _userAccountRepository.Update(userAccount); }
      catch (SqlException sqlEx)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, sqlEx.Message);
      }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult EditUserAccount(int userId)
    {
      if (HttpContext.Session.Get<bool>("_IsModerator") == false) return Redirect(Request.Host.ToString());

      var userEdit = new UserEditViewModel();
      var user = new UserAccount();
      var UserTypes = new Dictionary<string, int>();

      try { user = _userAccountRepository.GetById(userId); } catch (Exception) { }

      userEdit.Id = user.Id;
      userEdit.Active = user.Active;
      userEdit.CreatePost = user.CreatePost;
      userEdit.Name = user.Name;
      userEdit.Username = user.Username;
      userEdit.Email = user.Email.Substring(0, user.Email.IndexOf("@"));
      userEdit.Photo = user.Photo;
      userEdit.Description = user.Description;
      userEdit.DepartmentId = user.DepartmentId;

      try { userEdit.DepartmentDescription = _departmentRepository.GetDescriptionById(user.DepartmentId); } catch (Exception) { }

      if (user.Moderator == false && user.NoviceUser == false)
      {
        userEdit.UserType = "Comum";
        userEdit.UserTypeCode = 0;
      }
      else if (user.Moderator == false && user.NoviceUser == true)
      {
        userEdit.UserType = "Novato";
        userEdit.UserTypeCode = 1;
      }
      else
      {
        userEdit.UserType = "Moderador";
        userEdit.UserTypeCode = 2;
      }

      UserTypes.Add("Comum", 0);
      UserTypes.Add("Novato", 1);
      UserTypes.Add("Moderador", 2);
      userEdit.Novice = user.NoviceUser;
      userEdit.EditOwnerPost = user.EditOwnerPost;
      userEdit.EditAnyPost = user.EditAnyPost;
      userEdit.DeleteOwnerPost = user.DeleteOwnerPost;
      userEdit.DeleteAnyPost = user.DeleteAnyPost;
      userEdit.CreatePost = user.CreatePost;

      try { ViewBag.Departments = _departmentRepository.Get(); } catch (Exception) { }

      ViewBag.Permissions = UserTypes;
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjax("GET");

      return PartialView(userEdit);
    }

    [HttpPost]
    public IActionResult UpdateUserAccount(UserEditViewModel userEdited, List<IFormFile> Photo)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      var user = new UserAccount();
      user.Id = userEdited.Id;
      user.Active = userEdited.Active;
      user.CreatePost = userEdited.CreatePost;
      user.Name = userEdited.Name;
      user.Username = userEdited.Username;
      user.Email = userEdited.Email + "@engeman.com.br";
      user.Description = userEdited.Description;
      user.DepartmentId = userEdited.DepartmentId;

      if (userEdited.UserTypeCode == 1)
      {
        user.Moderator = false;
        user.NoviceUser = true;
        user.EditOwnerPost = true;
        user.DeleteOwnerPost = true;
        user.EditAnyPost = false;
        user.DeleteAnyPost = false;

      }
      else if (userEdited.UserTypeCode == 2)
      {
        user.Moderator = true;
        user.NoviceUser = false;
        user.EditOwnerPost = true;
        user.DeleteOwnerPost = true;
        user.EditAnyPost = true;
        user.DeleteAnyPost = true;
      }
      else
      {
        user.Moderator = false;
        user.NoviceUser = false;
        user.EditOwnerPost = true;
        user.DeleteOwnerPost = true;
        user.EditAnyPost = false;
        user.DeleteAnyPost = false;
      }

      if (Photo.Count == 0) try { user.Photo = _userAccountRepository.GetByUsername(user.Username).Photo; } catch (Exception) { }
      else
      {
        foreach (var item in Photo)
        {
          if (item.Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              item.CopyTo(stream);
              user.Photo = stream.ToArray();
            }
          }
        }
      }

      try { _userAccountRepository.UpdateByModerator(user.Id, user, sessionUsername); }
      catch (SqlException sqlEx)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, sqlEx.Message);
      }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public JsonResult DeleteUser(int userId)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string messageAux = null;
      int resultAux = StatusCodes.Status200OK;

      try { _userAccountRepository.Delete(userId, sessionUsername); }
      catch (SqlException sqlEx)
      {
        resultAux = StatusCodes.Status500InternalServerError;
        messageAux = sqlEx.Message;
      }

      return Json(new { result = resultAux, message = messageAux });
    }

    [HttpGet]
    public IActionResult CheckPermissions(int authorId, string method)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      string username = null;
      var permissions = new UserPermissionsViewModel();

      try
      {
        username = _userAccountRepository.GetUsernameById(authorId);
        permissions = _userAccountRepository.GetUserPermissionsByUsername(sessionUsername);
      }
      catch (Exception) { }

      if (method == "edit")
      {
        if (permissions.EditAnyPost == true) return Json("EditAnyPost");
        else if (permissions.EditOwnerPost == true)
        {
          if (username == sessionUsername) return Json("EditOwnerPost");
          else return Json("CannotEditAnyonePost");
        }
        else if (permissions.EditAnyPost == false && permissions.EditOwnerPost == false && username != sessionUsername) return Json("CannotEditAnyonePost");
        else if (permissions.EditAnyPost == false && permissions.EditOwnerPost == false) return Json("CannotEditOwnerPost");
      }
      else if (method == "delete")
      {
        if (permissions.DeleteAnyPost == true) return Json("DeleteAnyPost");
        else if (permissions.DeleteOwnerPost == true)
        {
          if (username == sessionUsername) return Json("DeleteOwnerPost");
          else return Json("CannotDeleteAnyonePost");
        }
        else if (permissions.DeleteAnyPost == false && permissions.DeleteOwnerPost == false && username != sessionUsername) return Json("CannotDeleteAnyonePost");
        else if (permissions.DeleteAnyPost == false && permissions.DeleteOwnerPost == false) return Json("CannotDeleteOwnerPost");
      }

      return Ok(StatusCodes.Status200OK);
    }
  }
}