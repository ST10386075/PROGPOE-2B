using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaims.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(UserRole? role, string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                Role = role ?? UserRole.Lecturer,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = _userService.ValidateUser(model.UserId, model.Password, model.Role);
                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for ID {UserId} and role {Role}", model.UserId, model.Role);
                    ModelState.AddModelError(string.Empty, "Invalid ID, password, or role.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("Email", user.Email ?? string.Empty),
                    new Claim("Phone", user.Phone ?? string.Empty),
                    new Claim("Department", user.Department ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                if (user.Role == UserRole.Lecturer)
                {
                    return RedirectToAction("Dashboard", "Lecturer");
                }

                return RedirectToAction("Dashboard", "ProgrammeCoordinator");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for ID {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while signing you in. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(UserRole? role)
        {
            var model = new RegisterViewModel
            {
                Role = role ?? UserRole.Lecturer
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var existing = _userService.GetById(model.UserId);
                if (existing != null)
                {
                    ModelState.AddModelError(nameof(model.UserId), "A user with this Staff ID already exists.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    Id = model.UserId,
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Department = model.Department,
                    Role = model.Role,
                    Password = model.Password
                };

                _userService.Create(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("Email", user.Email ?? string.Empty),
                    new Claim("Phone", user.Phone ?? string.Empty),
                    new Claim("Department", user.Department ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (user.Role == UserRole.Lecturer)
                {
                    return RedirectToAction("Dashboard", "Lecturer");
                }

                return RedirectToAction("Dashboard", "ProgrammeCoordinator");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for ID {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating your account. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login");
            }

            var user = _userService.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("Logout");
            }

            var model = new EditProfileViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Department = user.Department,
                Role = user.Role
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = _userService.GetById(model.UserId);
                if (user == null)
                {
                    TempData["Error"] = "We could not find your profile. Please sign in again.";
                    return RedirectToAction("Logout");
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Department = model.Department;

                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    user.Password = model.NewPassword;
                }

                _userService.Update(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("Email", user.Email ?? string.Empty),
                    new Claim("Phone", user.Phone ?? string.Empty),
                    new Claim("Department", user.Department ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["Success"] = "Your profile has been updated.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating profile for ID {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while updating your profile. Please try again.");
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
