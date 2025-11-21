using System.Linq;
using System.Security.Claims;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IContractMonthlyClaimService _claimService;
        private readonly ILogger<LecturerController> _logger;

        public LecturerController(IUserService userService, IContractMonthlyClaimService claimService, ILogger<LecturerController> logger)
        {
            _userService = userService;
            _claimService = claimService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var lecturerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(lecturerId))
                {
                    TempData["Error"] = "Your session has expired. Please sign in again.";
                    return RedirectToAction("Login", "Account");
                }

                var lecturer = _userService.GetById(lecturerId);
                if (lecturer == null)
                {
                    TempData["Error"] = "We could not find your lecturer profile. Please sign in again.";
                    return RedirectToAction("Logout", "Account");
                }

                var claims = _claimService.GetAll()
                    .Where(c => c.LecturerId == lecturerId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToList();

                var model = new LecturerDashboardViewModel
                {
                    Lecturer = lecturer,
                    RecentClaims = claims
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading lecturer dashboard for user {UserId}", User?.Identity?.Name);
                TempData["Error"] = "An unexpected error occurred while loading your dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
