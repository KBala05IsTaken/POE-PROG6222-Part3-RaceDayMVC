using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;
using System.Globalization;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : BaseController
    {
        private readonly RaceDayApiService _api;

        public OrganiserController(RaceDayApiService api)
        {
            _api = api;
        }

        // Dashboard: displays events, enrolment totals and upcoming dates.
        public async Task<IActionResult> Index()
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events") ?? new List<EventViewModel>();

            var dashboard = new DashboardViewModel
            {
                TotalEvents = events.Count,
                TotalEnrollments = events.Sum(e => e.TotalEnrollments),
                UpcomingEvents = events
                    .Where(e => e.EventDate.Date >= DateTime.Today)
                    .OrderBy(e => e.EventDate)
                    .ToList(),
                Events = events
            };

            return View(dashboard);
        }

        // Displays all events created by the organiser.
        public async Task<IActionResult> Events()
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events");

            return View(events ?? new List<EventViewModel>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            return View(new EventViewModel
            {
                EventDate = DateTime.Today.AddDays(1)
            });
        }

        // Creates an event and forwards its banner to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            if (!ModelState.IsValid)
                return View(model);

            var fields = new Dictionary<string, string>
            {
                ["EventName"] = model.EventName,
                ["EventDate"] = model.EventDate.ToString("o"),
                ["Location"] = model.Location,
                ["Description"] = model.Description ?? "",
                ["EventType"] = model.EventType
            };

            // The API endpoint must accept multipart form data.
            bool success = await _api.PostFileAsync(
                "api/organiser/events",
                fields,
                model.BannerImage,
                "BannerImage");

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "The event could not be created. Check the API endpoint.");
                return View(model);
            }

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction(nameof(Events));
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            var model = await _api.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id, EventViewModel model)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            if (!ModelState.IsValid)
                return View(model);

            var fields = new Dictionary<string, string>
            {
                ["EventName"] = model.EventName,
                ["EventDate"] = model.EventDate.ToString("o"),
                ["Location"] = model.Location,
                ["Description"] = model.Description ?? "",
                ["EventType"] = model.EventType
            };

            bool success = await _api.PutFileAsync(
                $"api/organiser/events/{id}",
                fields,
                model.BannerImage,
                "BannerImage");

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "The event could not be updated.");
                return View(model);
            }

            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction(nameof(Events));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            bool success = await _api.DeleteAsync(
                $"api/organiser/events/{id}");

            TempData[success ? "Success" : "Error"] =
                success ? "Event deleted." : "Could not delete event.";

            return RedirectToAction(nameof(Events));
        }

        // Displays categories belonging to a selected event.
        public async Task<IActionResult> Categories(int eventId)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            ViewBag.EventID = eventId;

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{eventId}");

            return View(categories ?? new List<CategoryViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please check the category details.";
                return RedirectToAction(
                    nameof(Categories),
                    new { eventId = model.EventID });
            }

            bool success = await _api.PostAsync(
                $"api/organiser/events/{model.EventID}/categories",
                model);

            TempData[success ? "Success" : "Error"] =
                success ? "Category added." : "Could not add category.";

            return RedirectToAction(
                nameof(Categories),
                new { eventId = model.EventID });
        }

        // Shows all participants enrolled in the selected event.
        public async Task<IActionResult> Enrollments(int eventId)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            ViewBag.EventID = eventId;

            var enrollments =
                await _api.GetAsync<List<EnrollmentViewModel>>(
                    $"api/organiser/events/{eventId}/enrollments");

            return View(enrollments ?? new List<EnrollmentViewModel>());
        }

        // Displays participants for whom results can be entered.
        public async Task<IActionResult> Results(int eventId)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            ViewBag.EventID = eventId;

            var enrollments =
                await _api.GetAsync<List<EnrollmentViewModel>>(
                    $"api/organiser/events/{eventId}/enrollments");

            return View(enrollments ?? new List<EnrollmentViewModel>());
        }

        // Saves or publishes a result for an enrolment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResult(
            int eventId,
            int enrollmentId,
            TimeSpan finishTime,
            int position,
            string? pace)
        {
            var access = CheckAccess("Organiser");
            if (access != null) return access;

            var result = new
            {
                FinishTime = finishTime,
                Position = position,
                Pace = pace
            };

            bool success = await _api.PostAsync(
                $"api/organiser/enrollments/{enrollmentId}/results",
                result);

            TempData[success ? "Success" : "Error"] =
                success ? "Result published." : "Could not save result.";

            return RedirectToAction(
                nameof(Results),
                new { eventId });
        }
    }
}
