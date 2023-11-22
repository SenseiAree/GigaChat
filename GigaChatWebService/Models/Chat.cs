using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class Chat
    {

        public int ChatId { get; set; }
        [Required]
        public int InitiatorId { get; set; }
        [Required]
        public int RecipientId { get; set; }
    }
}
