using Engeman.Intranet.Extensions;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices;
using System.Runtime.InteropServices;
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
      if (HttpContext.Session.Get<string>("_Username") == null) return PartialView();
      return RedirectToAction("index", "dashboard");
    }

    [HttpPost]
    [SupportedOSPlatform("windows")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TryLogin(LoginViewModel loginViewModel)
    {
      var userAccount = new UserAccount();

      //try
      //{
      //  DirectoryEntry entry = new("LDAP://" + _configuration["LocalPath"], loginViewModel.Username, loginViewModel.Password);
      //  Object obj = entry.NativeObject;
      //}
      //catch (COMException ex)
      //{
      //  TempData["Message"] = "Erro!" + "/" + ex.Message;
      //  return RedirectToAction("index", "login");
      //}

      try { userAccount = _userAccountRepository.GetByUsername(loginViewModel.Username); }
      catch (Exception) { }
      if (userAccount == null)
      {
        TempData["Message"] = "Erro!/Usuário não cadastrado.";
        return RedirectToAction("index", "login");
      }
      else
      {
        var claims = new List<Claim>
          {
            new Claim(ClaimTypes.Name, loginViewModel.Username)
          };
        var userIdentity = new ClaimsIdentity(claims, "Access");
        var principal = new ClaimsPrincipal(userIdentity);
        await HttpContext.SignInAsync("CookieAuthentication", principal, new AuthenticationProperties());
        HttpContext.Session.Set<int>("_CurrentUserId", userAccount.Id);
        HttpContext.Session.Set<int>("_DepartmentId", userAccount.DepartmentId);
        HttpContext.Session.Set<bool>("_Moderator", userAccount.Moderator);
        HttpContext.Session.Set<string>("_Username", loginViewModel.Username.ToString());
        return RedirectToAction("index", "dashboard");
      }
    }

    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync("CookieAuthentication");
      HttpContext.Session.Clear();
      Response.Cookies.Delete("UserSession");
      return RedirectToAction("index", "login");
    }
  }
}