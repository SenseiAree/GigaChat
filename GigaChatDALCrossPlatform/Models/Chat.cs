using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class Chat
    {
        public Chat()
        {
            Messages = new HashSet<Message>();
        }

        public int ChatId { get; set; }
        public int InitiatorId { get; set; }
        public int RecipientId { get; set; }

        public virtual User Initiator { get; set; }
        public virtual User Recipient { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
