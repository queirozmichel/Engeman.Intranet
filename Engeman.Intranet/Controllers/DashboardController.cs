using Engeman.Intranet.Models;
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

    public DashboardController(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IActionResult Index(Credentials credentials)
    {
      var url = _configuration["LocalPath"];      

      try
      {
        DirectoryEntry entry = new DirectoryEntry("LDAP://" + _configuration["LocalPath"], credentials.DomainUsername, credentials.Password);
        Object obj = entry.NativeObject;

      } catch (COMException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return View();
    }
  }
}
