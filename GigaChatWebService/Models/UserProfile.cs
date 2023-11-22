using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }
        [Required]
        public string Avatar { get; set; }
        [Required]
        public string IsActive { get; set; }
        [Required]
        public string Theme { get; set; }
        [Required]
        public string AvailabilityStatus { get; set; }
    }
}
