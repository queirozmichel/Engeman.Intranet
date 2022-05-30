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
    public async Task<IActionResult> TryLogin(string domainUsername, string password)
    {
      try
      {
        DirectoryEntry entry = new DirectoryEntry("LDAP://" + _configuration["LocalPath"], domainUsername, password);
        Object obj = entry.NativeObject;
      } catch (COMException ex)
      {
        TempData["Message"] = ex.Message;
        return RedirectToAction("index", "login");
      }

      if (_userAccountRepository.UserAccountValidate(domainUsername) == false)
      {
        TempData["Message"] = "Usuário não cadastrado ou bloqueado.";
        return RedirectToAction("index", "login");
      } else
      {
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, domainUsername));
        var userIdentity = new ClaimsIdentity(claims, "Access");
        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
        await HttpContext.SignInAsync("CookieAuthentication", principal, new AuthenticationProperties());
        HttpContext.Session.SetString("_DomainUsername", domainUsername.ToString());
        HttpContext.Session.SetString("_Password", password.ToString());
        return RedirectToAction("index", "dashboard");
      }
    }

    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync("CookieAuthentication");
      HttpContext.Session.Remove("_DomainUsername");
      HttpContext.Session.Remove("_Password");
      return RedirectToAction("index", "login");
    }
  }
}