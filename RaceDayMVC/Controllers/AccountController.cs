using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly RaceDayApiService _api;

        public AccountController(RaceDayApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var registrationData = new
            {
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.Role
            };

            // Change this URL to your actual API registration endpoint.
            bool success = await _api.PostAsync(
                "api/auth/register",
                registrationData);

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "Registration failed. Check your details or try another email.");

                return View(model);
            }

            TempData["Success"] = "Registration successful. Please log in.";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginData = new
            {
                model.Email,
                model.Password
            };

            // Change this URL to your actual API login endpoint.
            var result = await _api.PostAsync<object, AuthResponse>(
                "api/auth/login",
                loginData);

            if (result == null ||
                string.IsNullOrWhiteSpace(result.Token))
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }

            // Save the authenticated user's information in session.
            HttpContext.Session.SetString("AuthToken", result.Token);
            HttpContext.Session.SetString("UserID", result.UserID.ToString());
            HttpContext.Session.SetString("FirstName", result.FirstName);
            HttpContext.Session.SetString("LastName", result.LastName);
            HttpContext.Session.SetString("Email", result.Email);
            HttpContext.Session.SetString("Role", result.Role);

            if (result.Role == "Organiser")
                return RedirectToAction("Index", "Organiser");

            return RedirectToAction("Index", "Participant");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
