using ContactList.Web.Common.Services;
using ContactList.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ContactList.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRedisService _redisService;
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, IRedisService redisService, ILogger<AccountController> logger)
        {
            _redisService = redisService;
            _authService = authService;
            _logger = logger;
        }

        // Register: Show registration form
        public IActionResult Register() => View();

        // Register: Handle user registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["ErrorMessage"] = "Username and password are required.";
                    return View();
                }

                var success = _authService.Register(username, password);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Registration failed. Username might already exist.";
                    return View();
                }

                await _redisService.StoreDataAsync("UserName", username);
                _logger.LogInformation("User registered successfully: {Username}", username);

                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", username);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                return View();
            }
        }




        // Login: Show login form
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to the Contacts Index page if already logged in
                return RedirectToAction("Index", "Contacts");
            }

            return View();
        }

        // Login: Handle user authentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Username and password are required.";
                    return View();
                }

                var user = _authService.Authenticate(username, password);
                if (user == null)
                {
                    ViewBag.Error = "Invalid credentials.";
                    _logger.LogWarning("Failed login attempt for user: {Username}", username);
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("UserId", user.UserId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                _logger.LogInformation("User logged in successfully: {Username}", username);
                return RedirectToAction("Index", "Contacts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", username);
                return StatusCode(500, "An error occurred while processing your login.");
            }
        }

        // Logout: Sign out user
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign the user out and remove their authentication cookie
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("User logged out successfully.");

                // Redirect to the login page
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout.");
                return StatusCode(500, "An error occurred while processing your logout.");
            }
        }


        // Reset: Show the password reset form
        public IActionResult Reset()
        {
            try
            {
                //// You can add logic to ensure that only authenticated users can access this action
                //if (!User.Identity.IsAuthenticated)
                //{
                //    _logger.LogWarning("Unauthorized access attempt to Reset Password.");
                //    return RedirectToAction("Login", "Account");
                //}

                //// Optionally, you could retrieve the current user if needed, like fetching the username
                //var userId = User.FindFirstValue("UserId");
                //if (userId == null)
                //{
                //    _logger.LogWarning("Unauthorized access attempt to Reset Password. User not found.");
                //    return RedirectToAction("Login", "Account");
                //}

                // Render the reset password view
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while displaying the password reset form.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        // Reset Password: Handle password reset
        // Reset: Handle password reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset(string username, string newPassword, string confirmPassword)
        {
            try
            {
                // Check if the username, new password, or confirm password is empty
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ViewBag.Error = "Username and both password fields are required.";
                    return View();
                }

                // Ensure that the new password and confirmation password match
                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                // Validate the password strength if needed (you can add custom checks here)
                if (newPassword.Length < 8 || !newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
                {
                    ViewBag.Error = "Password must be at least 8 characters long and contain both letters and numbers.";
                    return View();
                }

                // Call the service to reset the password (you may need to update your service method)
                var success = _authService.ResetPassword(username, newPassword);
                if (!success)
                {
                    ViewBag.Error = "User not found or password reset failed.";
                    _logger.LogWarning("Password reset failed for user: {Username}", username);
                    return View();
                }

                ViewBag.Success = "Your password has been reset successfully. You can now log in with your new password.";
                _logger.LogInformation("Password reset successfully for user: {Username}", username);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for user: {Username}", username);
                return StatusCode(500, "An error occurred while processing your password reset.");
            }
        }

    }
}
