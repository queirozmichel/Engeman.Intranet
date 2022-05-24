using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Engeman.Intranet.Controllers
{
  public class DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
