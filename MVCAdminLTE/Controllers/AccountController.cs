using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MSS.Api.Services;
using MVCAdminLTE.ApiServices;
using MVCAdminLTE.Models;
using System.Security.Claims;

namespace MVCAdminLTE.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserApiService _userApiService;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(UserApiService userApiService, IPasswordHasher passwordHasher)
        {
            _userApiService = userApiService;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var user = await _userApiService.GetByUsernameAsync(model.Username);
                if (user == null || !user.IsActive)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    ViewBag.Error = "Invalid username or password.";
                    return View("Index", model);
                }

                if (!_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    ViewBag.Error = "Invalid username or password.";
                    return View("Index", model);
                }

                user = await _userApiService.GetUserWithRolesAndPermissionsAsync(user.Id);
                if (user == null)
                {
                    ModelState.AddModelError("", "Error loading user data.");
                    ViewBag.Error = "Error loading user data.";
                    return View("Index", model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                foreach (var permission in user.Permissions)
                {
                    claims.Add(new Claim("Permission", permission.Name));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                ViewBag.Error = "An error occurred during login. Please try again.";
                return View("Index", model);
            }
        }
    }
}
