using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data.Entities.Identity;
using UrlShortener.MVC.Models;

namespace UrlShortener.MVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<UrlShortenerUser> _signInManager;
        private readonly UserManager<UrlShortenerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            UserManager<UrlShortenerUser> userManager,
            SignInManager<UrlShortenerUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Authentication/Login
        public IActionResult Login(string? returnUrl = null)
        {
            var loginVM = new LoginVM
            {
                ReturnUrl = returnUrl
            };
            return View(loginVM);
        }

        // POST: Authentication/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            try
            {
                var returnUrl = loginVM.ReturnUrl ?? Url.Content("~/");

                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        loginVM.Email, 
                        loginVM.Password, 
                        loginVM.RememberMe, 
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginVM.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToAction("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(loginVM);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
            }
            
            return View(loginVM);
        }

        // GET: Authentication/Register
        public IActionResult Register(string? returnUrl = null)
        {
            var registerVM = new RegisterVM
            {
                ReturnUrl = returnUrl
            };
            return View(registerVM);
        }

        // POST: Authentication/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            try
            {
                var returnUrl = string.IsNullOrEmpty(registerVM.ReturnUrl) ? Url.Content("~/") : registerVM.ReturnUrl;

                if (ModelState.IsValid)
                {
                    var user = new UrlShortenerUser
                    {
                        UserName = registerVM.Email,
                        Email = registerVM.Email,
                        FullName = registerVM.FullName,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(user, registerVM.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // Assign "User" role by default
                        await EnsureRolesExist();
                        await _userManager.AddToRoleAsync(user, "User");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ModelState.AddModelError(string.Empty, "An error occurred during registration.");
            }

            return View(registerVM);
        }

        // GET: Authentication/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "UrlShortener");
            }
        }

        // Ensure Admin and User roles exist
        private async Task EnsureRolesExist()
        {
            string[] roleNames = { "Admin", "User" };
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // Create Admin User (call this manually or via seed data)
        public async Task<IActionResult> CreateAdminUser()
        {
            await EnsureRolesExist();

            var adminEmail = "admin@urlshortener.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new UrlShortenerUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return Content("Admin user created successfully! Email: admin@urlshortener.com, Password: Admin@123");
                }
                else
                {
                    return Content("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            return Content("Admin user already exists!");
        }
    }
}
