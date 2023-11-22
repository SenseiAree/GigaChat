using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        [Required]
        public int ChatId { get; set; }
        [Required] 
        public int SenderId { get; set; }
        [Required] 
        public int ReceiverId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required] 
        public DateTime SentTime { get; set; }
        public string IsRead { get; set; }
    }
}
