using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : BaseController
    {
        private readonly RaceDayApiService _api;

        public ParticipantController(RaceDayApiService api)
        {
            _api = api;
        }

        // Participant home page with event filtering.
        public async Task<IActionResult> Index(
            string? search,
            string? eventType)
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events") ?? new List<EventViewModel>();

            // Only show upcoming events.
            events = events
                .Where(e => e.EventDate.Date >= DateTime.Today)
                .ToList();

            // Apply the search filter.
            if (!string.IsNullOrWhiteSpace(search))
            {
                events = events.Where(e =>
                    e.EventName.Contains(
                        search, StringComparison.OrdinalIgnoreCase) ||
                    e.Location.Contains(
                        search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Apply the event-type filter.
            if (!string.IsNullOrWhiteSpace(eventType))
            {
                events = events.Where(e =>
                    e.EventType.Equals(
                        eventType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Search = search;
            ViewBag.EventType = eventType;

            return View(events);
        }

        // Displays one event and its available categories.
        public async Task<IActionResult> EventDetails(int id)
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var eventDetails = await _api.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventDetails == null)
                return NotFound();

            return View(eventDetails);
        }

        // Sends the participant's selected category to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int eventId, int categoryId)
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var request = new
            {
                EventID = eventId,
                CategoryID = categoryId
            };

            bool success = await _api.PostAsync(
                "api/participant/enrolments",
                request);

            TempData[success ? "Success" : "Error"] =
                success
                    ? "You have successfully enrolled."
                    : "Enrolment failed. Please check the event and category.";

            return RedirectToAction(
                nameof(EventDetails),
                new { id = eventId });
        }

        // Displays the participant's own enrolments.
        public async Task<IActionResult> MyEnrollments()
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var userId = HttpContext.Session.GetString("UserID");

            var enrolments =
                await _api.GetAsync<List<EnrollmentViewModel>>(
                    $"api/participant/enrolments/{userId}");

            return View(enrolments ?? new List<EnrollmentViewModel>());
        }

        // Displays the participant's race history.
        public async Task<IActionResult> MyResults()
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var userId = HttpContext.Session.GetString("UserID");

            var results = await _api.GetAsync<List<ResultViewModel>>(
                $"api/participant/results/{userId}");

            return View(results ?? new List<ResultViewModel>());
        }

        // Gets the participant's current profile from the API.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            var userId = HttpContext.Session.GetString("UserID");

            var profile = await _api.GetAsync<ProfileViewModel>(
                $"api/profile/{userId}");

            if (profile == null)
            {
                // Use session values if the API profile route is unavailable.
                profile = new ProfileViewModel
                {
                    UserID = int.Parse(userId!),
                    FirstName = HttpContext.Session.GetString("FirstName") ?? "",
                    LastName = HttpContext.Session.GetString("LastName") ?? "",
                    Email = HttpContext.Session.GetString("Email") ?? ""
                };
            }

            return View(profile);
        }

        // Updates personal information and optionally uploads a profile picture.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var access = CheckAccess("Participant");
            if (access != null) return access;

            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetString("UserID");

            var fields = new Dictionary<string, string>
            {
                ["FirstName"] = model.FirstName,
                ["LastName"] = model.LastName,
                ["Email"] = model.Email,
                ["NewPassword"] = model.NewPassword ?? ""
            };

            // The API must accept multipart data and update the profile.
            bool success = await _api.PutFileAsync(
                $"api/profile/{userId}",
                fields,
                model.ProfilePicture,
                "ProfilePicture");

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "Your profile could not be updated.");
                return View(model);
            }

            // Keep the displayed session information up to date.
            HttpContext.Session.SetString("FirstName", model.FirstName);
            HttpContext.Session.SetString("LastName", model.LastName);
            HttpContext.Session.SetString("Email", model.Email);

            TempData["Success"] = "Your profile has been updated.";

            return RedirectToAction(nameof(Profile));
        }
    }
}
