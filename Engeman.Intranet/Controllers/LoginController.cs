using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

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
    public IActionResult TryLogin(CredentialsDto credentials)
    {
      try
      {
        DirectoryEntry entry = new DirectoryEntry("LDAP://" + _configuration["LocalPath"], credentials.DomainUsername, credentials.Password);
        Object obj = entry.NativeObject;
      } catch (COMException ex)
      {
        TempData["Message"] = ex.Message;
        //return PartialView("~/Views/Login/Index.cshtml");
        return RedirectToAction("index", "login");
      }

      if (_userAccountRepository.UserAccountValidate(credentials.DomainUsername) == false)
      {
        TempData["Message"] = "Usuário não cadastrado ou bloqueado.";
        //return PartialView("~/Views/Login/Index.cshtml");
        return RedirectToAction("index", "login");
      } else
      {
        HttpContext.Session.SetString("_DomainUsername", credentials.DomainUsername.ToString());
        HttpContext.Session.SetString("_Password", credentials.Password.ToString());
        //return View("~/Views/dashboard/index.cshtml");
        return RedirectToAction("index", "dashboard");
      }
    }
  }
}