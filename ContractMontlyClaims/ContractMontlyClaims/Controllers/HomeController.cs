using System.Diagnostics;
using ContractMontlyClaims.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Lecturer"))
                {
                    return RedirectToAction("Dashboard", "Lecturer");
                }

                if (User.IsInRole("ProgrammeCoordinator"))
                {
                    return RedirectToAction("Dashboard", "ProgrammeCoordinator");
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
