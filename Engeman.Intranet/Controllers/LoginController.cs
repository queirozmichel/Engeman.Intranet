using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace Engeman.Intranet.Controllers
{
  public class LoginController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly IUserAccountRepository _userAccountRepository;

    public LoginController(IConfiguration configuration, IUserAccountRepository userAccountRepository)
    {
      _configuration = configuration;
      _userAccountRepository = userAccountRepository;
    }

    public IActionResult Index()
    {
      if (HttpContext.Session.GetString("_Username") == null)
      {
        return PartialView();
      }
      return RedirectToAction("index", "dashboard");
    }

    [HttpPost]
    [SupportedOSPlatform("windows")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TryLogin(LoginViewModel loginViewModel)
    {
      if (!ModelState.IsValid)
      {
        return RedirectToAction("Error");
      }
      else
      {
        //try
        //{
        //DirectoryEntry entry = new("LDAP://" + _configuration["LocalPath"], loginViewModel.Username, loginViewModel.Password);
        //  Object obj = entry.NativeObject;
        //}
        //catch (COMException ex)
        //{
        //  TempData["Message"] = ex.Message;
        //  return RedirectToAction("index", "login");
        //}

        var user = _userAccountRepository.GetByUsername(loginViewModel.Username);

        if (user == null)
        {
          TempData["Message"] = "Usuário não cadastrado ou bloqueado.";
          return RedirectToAction("index", "login");
        }
        else
        {
          var userAccount = _userAccountRepository.GetByUsername(loginViewModel.Username);
          var claims = new List<Claim>
          {
            new Claim(ClaimTypes.Name, loginViewModel.Username)
          };
          var userIdentity = new ClaimsIdentity(claims, "Access");
          ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
          await HttpContext.SignInAsync("CookieAuthentication", principal, new AuthenticationProperties());

          HttpContext.Session.SetInt32("_UserAccountId", userAccount.Id);
          Response.Cookies.Append("_UserId", userAccount.Id.ToString());
          HttpContext.Session.SetInt32("_DepartmentId", userAccount.DepartmentId);
          HttpContext.Session.SetInt32("_Moderator", Convert.ToInt32(userAccount.Moderator));
          HttpContext.Session.SetString("_Username", loginViewModel.Username.ToString());
          HttpContext.Session.SetString("_Password", loginViewModel.Password.ToString());
          return RedirectToAction("index", "dashboard");
        }
      }
    }

    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync("CookieAuthentication");
      HttpContext.Session.Clear();
      Response.Cookies.Delete("_UserId");
      Response.Cookies.Delete("UserSession");
      return RedirectToAction("index", "login");
    }
  }
}