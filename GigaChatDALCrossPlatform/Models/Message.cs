using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class Message
    {
        public Message()
        {
            StarredMessages = new HashSet<StarredMessage>();
        }

        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public string IsRead { get; set; }

        public virtual Chat Chat { get; set; }
        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
        public virtual ICollection<StarredMessage> StarredMessages { get; set; }
    }
}
