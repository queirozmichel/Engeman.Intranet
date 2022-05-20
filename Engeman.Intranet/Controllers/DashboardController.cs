using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Engeman.Intranet.Controllers
{
  public class DashboardController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly IUserAccountRepository _userAccountRepository;

    public DashboardController(IConfiguration configuration, IUserAccountRepository userAccountRepository)
    {
      _configuration = configuration;
      _userAccountRepository = userAccountRepository;
    }

    public IActionResult Index(Credentials credentials)
    {
      try
      {
        DirectoryEntry entry = new DirectoryEntry("LDAP://" + _configuration["LocalPath"], credentials.DomainUsername, credentials.Password);
        Object obj = entry.NativeObject;

      } catch (COMException ex)
      {
        ViewData["Message"] = ex.Message;
        return PartialView("~/Views/Login/Index.cshtml");
      }     

      if (_userAccountRepository.UserAccountValidate(credentials) == false)
      {
        ViewData["Message"] = "O usuário não existe.";
        return PartialView("~/Views/Login/Index.cshtml");
      } else
      {
        return View();
      }
    }
  }
}
