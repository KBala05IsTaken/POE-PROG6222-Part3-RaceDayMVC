namespace RaceDayMVC.Models
{
    public class EnrollmentViewModel
    {
        public int EnrollmentID { get; set; }

        public int UserID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public string EventName { get; set; } = "";

        public string ParticipantName { get; set; } = "";

        public string CategoryName { get; set; } = "";

        public DateTime EventDate { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; } = "";
    }
}
