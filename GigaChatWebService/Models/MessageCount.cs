using System.ComponentModel.DataAnnotations;

namespace GigaChatWebService.Models
{
    public class MessageCount
    {
        [Required]
        public int SenderId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
