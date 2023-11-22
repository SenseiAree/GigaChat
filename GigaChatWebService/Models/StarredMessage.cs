using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class StarredMessage
    {
        public int StarredMessageId { get; set; }
        [Required]
        public int MessageId { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
