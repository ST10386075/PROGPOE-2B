using System;
using System.Linq;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    [Authorize(Roles = "ProgrammeCoordinator")]
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly IContractMonthlyClaimService _claimService;
        private readonly ILogger<ProgrammeCoordinatorController> _logger;

        public ProgrammeCoordinatorController(IContractMonthlyClaimService claimService, ILogger<ProgrammeCoordinatorController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var allClaims = _claimService.GetAll();
                var pending = allClaims.Where(c => c.Status == ClaimStatus.Pending).ToList();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var claimsThisMonth = allClaims.Where(c => c.CreatedAt >= startOfMonth).ToList();

                var model = new CoordinatorDashboardViewModel
                {
                    PendingCount = pending.Count,
                    ClaimsThisMonth = claimsThisMonth.Count,
                    AmountPending = pending.Sum(c => c.TotalAmount),
                    ApprovedThisMonth = claimsThisMonth.Count(c => c.Status == ClaimStatus.Approved),
                    PendingClaims = pending
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading programme coordinator dashboard");
                TempData["Error"] = "An unexpected error occurred while loading the coordinator dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
