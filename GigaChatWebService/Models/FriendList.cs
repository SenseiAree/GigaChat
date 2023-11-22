using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class FriendList
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int FriendId { get; set; }
        [Required]
        public bool IsBlocked { get; set; }
    }
}
