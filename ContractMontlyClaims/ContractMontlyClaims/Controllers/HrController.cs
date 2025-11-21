using System;
using System.Linq;
using System.Text;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    public class HrController : Controller
    {
        private readonly IContractMonthlyClaimService _claimService;
        private readonly ILogger<HrController> _logger;

        public HrController(IContractMonthlyClaimService claimService, ILogger<HrController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var claims = _claimService.GetAll();

                var approved = claims.Where(c => c.Status == ClaimStatus.Approved).ToList();
                var settled = claims.Where(c => c.Status == ClaimStatus.Settled).ToList();

                var model = new HrDashboardViewModel
                {
                    ApprovedCount = approved.Count,
                    ApprovedTotal = approved.Sum(c => c.TotalAmount),
                    SettledCount = settled.Count,
                    SettledTotal = settled.Sum(c => c.TotalAmount),
                    RecentApproved = approved.OrderByDescending(c => c.CreatedAt).Take(10).ToList(),
                    RecentSettled = settled.OrderByDescending(c => c.CreatedAt).Take(10).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading HR dashboard");
                TempData["Error"] = "An unexpected error occurred while loading the HR dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult ExportApproved()
        {
            try
            {
                var claims = _claimService.GetAll()
                    .Where(c => c.Status == ClaimStatus.Approved || c.Status == ClaimStatus.Settled)
                    .OrderBy(c => c.CreatedAt)
                    .ToList();

                var builder = new StringBuilder();
                builder.AppendLine("Id,Status,HoursWorked,HourlyRate,TotalAmount,CreatedAt,UpdatedAt");

                foreach (var claim in claims)
                {
                    builder.Append(claim.Id).Append(',');
                    builder.Append(claim.Status).Append(',');
                    builder.Append(claim.HoursWorked).Append(',');
                    builder.Append(claim.HourlyRate).Append(',');
                    builder.Append(claim.TotalAmount).Append(',');
                    builder.Append(claim.CreatedAt.ToString("yyyy-MM-dd HH:mm")).Append(',');
                    builder.AppendLine(claim.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty);
                }

                var bytes = Encoding.UTF8.GetBytes(builder.ToString());
                var fileName = "claims-report-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmm") + ".csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while exporting approved claims report");
                TempData["Error"] = "An unexpected error occurred while generating the report.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
