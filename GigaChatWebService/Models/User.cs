using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class User
    {
        [Required]
        public int UserId { get; set; }
        [Required] 
        public string EmailId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
