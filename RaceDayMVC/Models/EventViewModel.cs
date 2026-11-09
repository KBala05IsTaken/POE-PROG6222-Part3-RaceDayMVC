using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; } = "";

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = "";

        public string? Description { get; set; }

        // Running, Walking or Cycling.
        [Required]
        public string EventType { get; set; } = "";

        // The API should return the saved banner URL.
        public string? BannerImageUrl { get; set; }

        // Used when creating or editing an event.
        // The MVC application forwards this file to the API.
        public IFormFile? BannerImage { get; set; }

        public int TotalEnrollments { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
