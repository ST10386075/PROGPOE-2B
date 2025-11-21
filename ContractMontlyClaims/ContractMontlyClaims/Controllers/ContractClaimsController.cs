using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    public class ContractClaimsController : Controller
    {
        private readonly IContractMonthlyClaimService _claimService;
        private readonly ILogger<ContractClaimsController> _logger;
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };

        public ContractClaimsController(
            IContractMonthlyClaimService claimService,
            ILogger<ContractClaimsController> logger,
            IWebHostEnvironment environment)
        {
            _claimService = claimService;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create()
        {
            return View(new ContractMonthlyClaim());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create(ContractMonthlyClaim model, IFormFile? attachment)
        {
            if (attachment != null && attachment.Length > 0)
            {
                if (attachment.Length > MaxFileSizeBytes)
                {
                    ModelState.AddModelError(string.Empty, "The uploaded file is too large. Maximum size is 5 MB.");
                }
                else
                {
                    var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();
                    if (!AllowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(string.Empty, "Only .pdf, .docx, and .xlsx files are allowed.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (attachment != null && attachment.Length > 0)
            {
                try
                {
                    var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsRoot);

                    var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();
                    var fileName = Guid.NewGuid().ToString("N") + extension;
                    var filePath = Path.Combine(uploadsRoot, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await attachment.CopyToAsync(stream);
                    }

                    model.AttachmentFileName = fileName;
                    model.AttachmentOriginalFileName = Path.GetFileName(attachment.FileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while saving attachment for a claim");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while uploading the file. Please try again.");
                    return View(model);
                }
            }

            var lecturerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecturerName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(lecturerId))
            {
                TempData["Error"] = "Your session has expired. Please sign in again.";
                return RedirectToAction("Login", "Account");
            }

            model.LecturerId = lecturerId;
            model.LecturerName = lecturerName;

            try
            {
                _claimService.Create(model);
                TempData["Success"] = "Your claim has been submitted successfully.";
                return RedirectToAction(nameof(MyClaims));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new claim");
                TempData["Error"] = "An unexpected error occurred while submitting your claim. Please try again.";
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public IActionResult MyClaims()
        {
            var lecturerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(lecturerId))
            {
                TempData["Error"] = "Your session has expired. Please sign in again.";
                return RedirectToAction("Login", "Account");
            }

            var claims = _claimService.GetAll()
                .Where(c => c.LecturerId == lecturerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return View(claims);
        }

        [HttpGet]
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Pending()
        {
            var claims = _claimService.GetPending();
            return View(claims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Approve(int id)
        {
            try
            {
                var updated = _claimService.UpdateStatus(id, ClaimStatus.Approved);
                if (!updated)
                {
                    TempData["Error"] = "The claim you tried to approve could not be found.";
                }
                else
                {
                    TempData["Success"] = "Claim has been approved.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while approving claim {Id}", id);
                TempData["Error"] = "An unexpected error occurred while approving the claim.";
            }

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Reject(int id)
        {
            try
            {
                var updated = _claimService.UpdateStatus(id, ClaimStatus.Rejected);
                if (!updated)
                {
                    TempData["Error"] = "The claim you tried to reject could not be found.";
                }
                else
                {
                    TempData["Success"] = "Claim has been rejected.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while rejecting claim {Id}", id);
                TempData["Error"] = "An unexpected error occurred while rejecting the claim.";
            }

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Settle(int id)
        {
            try
            {
                var updated = _claimService.UpdateStatus(id, ClaimStatus.Settled);
                if (!updated)
                {
                    TempData["Error"] = "The claim you tried to mark as settled could not be found.";
                }
                else
                {
                    TempData["Success"] = "Claim has been marked as settled.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while settling claim {Id}", id);
                TempData["Error"] = "An unexpected error occurred while marking the claim as settled.";
            }

            return RedirectToAction(nameof(Pending));
        }

        [HttpGet]
        [Authorize]
        public IActionResult DownloadAttachment(int id)
        {
            try
            {
                var claim = _claimService.GetById(id);
                if (claim == null || string.IsNullOrWhiteSpace(claim.AttachmentFileName))
                {
                    TempData["Error"] = "The requested attachment could not be found.";
                    return RedirectToSafeClaimsPage();
                }

                var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsRoot, claim.AttachmentFileName);

                if (!System.IO.File.Exists(filePath))
                {
                    TempData["Error"] = "The requested attachment is no longer available on the server.";
                    return RedirectToSafeClaimsPage();
                }

                var extension = Path.GetExtension(claim.AttachmentFileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                var downloadName = string.IsNullOrWhiteSpace(claim.AttachmentOriginalFileName)
                    ? "attachment" + extension
                    : claim.AttachmentOriginalFileName;

                return PhysicalFile(filePath, contentType, downloadName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while downloading attachment for claim {Id}", id);
                TempData["Error"] = "An unexpected error occurred while downloading the attachment.";
                return RedirectToSafeClaimsPage();
            }
        }

        private IActionResult RedirectToSafeClaimsPage()
        {
            if (User.IsInRole("Lecturer"))
            {
                return RedirectToAction(nameof(MyClaims));
            }

            if (User.IsInRole("ProgrammeCoordinator"))
            {
                return RedirectToAction(nameof(Pending));
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
