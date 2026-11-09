namespace RaceDayMVC.Models
{
    public class DashboardViewModel
    {
        public int TotalEvents { get; set; }

        public int TotalEnrollments { get; set; }

        public List<EventViewModel> UpcomingEvents { get; set; } = new();

        public List<EventViewModel> Events { get; set; } = new();
    }
}
