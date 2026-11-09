namespace RaceDayMVC.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public int UserID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}