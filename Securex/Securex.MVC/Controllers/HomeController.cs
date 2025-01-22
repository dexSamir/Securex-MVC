using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Securex.MVC.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}

