using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class StarredMessage
    {
        public int StarredMessageId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }

        public virtual Message Message { get; set; }
        public virtual User User { get; set; }
    }
}
