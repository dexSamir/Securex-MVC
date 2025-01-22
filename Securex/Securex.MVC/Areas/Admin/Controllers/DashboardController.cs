using Microsoft.AspNetCore.Mvc;

namespace Securex.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class DashboardController : Controller
{
    // GET: DashboardController
    public ActionResult Index()
    {
        return View();
    }

}
