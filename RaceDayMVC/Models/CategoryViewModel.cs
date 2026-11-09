using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        [Required]
        public string CategoryName { get; set; } = "";

        [Range(0.1, 1000)]
        public decimal DistanceKM { get; set; }

        [Range(1, 100000)]
        public int MaximumParticipants { get; set; }

        [Range(0, 100000)]
        public decimal EntryFee { get; set; }
    }
}