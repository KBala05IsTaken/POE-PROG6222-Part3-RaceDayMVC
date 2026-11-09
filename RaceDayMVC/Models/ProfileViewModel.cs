using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        // Optional new password.
        public string? NewPassword { get; set; }

        // Optional profile picture upload.
        public IFormFile? ProfilePicture { get; set; }

        // URL returned by the API.
        public string? ProfilePictureUrl { get; set; }
    }
}
