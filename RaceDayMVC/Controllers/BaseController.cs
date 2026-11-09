using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayMVC.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("AuthToken"));
        }

        protected bool HasRole(string role)
        {
            return HttpContext.Session.GetString("Role") == role;
        }

        protected IActionResult? CheckAccess(string? requiredRole = null)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (requiredRole != null && !HasRole(requiredRole))
                return RedirectToAction("AccessDenied", "Home");

            return null;
        }
    }
}
