using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Threading.Tasks;

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
      return PartialView();
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
        //  DirectoryEntry entry = new("LDAP://" + _configuration["LocalPath"], loginViewModel.DomainAccount, loginViewModel.Password);
        //  Object obj = entry.NativeObject;
        //}
        //catch (COMException ex)
        //{
        //  TempData["Message"] = ex.Message;
        //  return RedirectToAction("index", "login");
        //}

        if (_userAccountRepository.UserAccountValidate(loginViewModel.DomainAccount) == false)
        {
          TempData["Message"] = "Usuário não cadastrado ou bloqueado.";
          return RedirectToAction("index", "login");
        }
        else
        {
          var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(loginViewModel.DomainAccount);
          var claims = new List<Claim>();
          claims.Add(new Claim(ClaimTypes.Name, loginViewModel.DomainAccount));
          var userIdentity = new ClaimsIdentity(claims, "Access");
          ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
          await HttpContext.SignInAsync("CookieAuthentication", principal, new AuthenticationProperties());

          HttpContext.Session.SetInt32("_Id", userAccount.Id);
          HttpContext.Session.SetInt32 ("_DepartmentId", userAccount.DepartmentId);
          HttpContext.Session.SetString("_DomainUsername", loginViewModel.DomainAccount.ToString());
          HttpContext.Session.SetString("_Password", loginViewModel.Password.ToString());
          return RedirectToAction("index", "dashboard");
        }
      }
    }
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync("CookieAuthentication");
      HttpContext.Session.Remove("_Id");
      HttpContext.Session.Remove("_DepartmentId");
      HttpContext.Session.Remove("_DomainUsername");
      HttpContext.Session.Remove("_Password");
      return RedirectToAction("index", "login");
    }

    [HttpGet]
    public JsonResult ConfirmSessionUserByAjax(int userAccountIdPost)
    {
      var userSessionId = HttpContext.Session.GetInt32("_Id");

      if (userSessionId == userAccountIdPost)
      {
        return Json(true);
      }
      else
      {
        return Json(false);
      }
    }
  }
}