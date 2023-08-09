using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Engeman.Intranet.Models;
using Microsoft.AspNetCore.Authorization;
using Engeman.Intranet.Models.ViewModels;
using System.Linq.Dynamic.Core;
using Engeman.Intranet.Extensions;
using System.Data.SqlClient;
using System.Text.Json;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class UserAccountController : RootController
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

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");

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

      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");
      ViewBag.Permissions = GetPermissions().Value;

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

      try { user = _userAccountRepository.GetById(userId); } catch (Exception) { }

      userEdit.Id = user.Id;
      userEdit.Active = user.Active;
      userEdit.Name = user.Name;
      userEdit.Username = user.Username;
      userEdit.Email = user.Email.Substring(0, user.Email.IndexOf("@"));
      userEdit.Photo = user.Photo;
      userEdit.Description = user.Description;
      userEdit.DepartmentId = user.DepartmentId;

      try
      {
        userEdit.DepartmentDescription = _departmentRepository.GetDescriptionById(user.DepartmentId);
        ViewBag.Departments = _departmentRepository.Get();
      }
      catch (Exception) { }

      ViewBag.Permissions = GetPermissions(userId).Value;
      ViewBag.IsAjaxCall = HttpContext.Request.IsAjaxOrFetch("GET");

      return PartialView(userEdit);
    }

    [HttpPost]
    public IActionResult UpdateUserAccount(UserEditViewModel userEdited, List<IFormFile> Photo)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      var user = new UserAccount();
      user.Id = userEdited.Id;
      user.Active = userEdited.Active;
      user.Name = userEdited.Name;
      user.Username = userEdited.Username;
      user.Email = userEdited.Email + "@engeman.com.br";
      user.Description = userEdited.Description;
      user.DepartmentId = userEdited.DepartmentId;
      user.Permissions = userEdited.Permissions.SerializeAndBoolToIntConverter();

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
    public JsonResult GetPermissions(int userId = 0)
    {
      string aux;
      UserPermissionsViewModel permissions = new();

      if (userId == 0)
      {
        try { aux = _userAccountRepository.GetPermissionsById(HttpContext.Session.Get<int>("_CurrentUserId")); }
        catch (Exception) { throw; }
      }
      else
      {
        try { aux = _userAccountRepository.GetPermissionsById(userId); }
        catch (Exception) { throw; }
      }

      permissions = aux.DeserializeAndConvertIntToBool<UserPermissionsViewModel>();

      return Json(permissions);
    }
  }
}