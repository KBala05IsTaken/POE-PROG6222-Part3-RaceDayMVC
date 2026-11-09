namespace RaceDayMVC.Models
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrollmentID { get; set; }

        public string EventName { get; set; } = "";

        public DateTime EventDate { get; set; }

        public string CategoryName { get; set; } = "";

        public string ParticipantName { get; set; } = "";

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string? Pace { get; set; }

        public DateTime? ResultDate { get; set; }
    }
}