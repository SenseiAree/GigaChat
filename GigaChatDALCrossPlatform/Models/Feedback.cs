using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string UserFeedback { get; set; }
        public int Rating { get; set; }
        public DateTime PostedAt { get; set; }
        public string AdminReply { get; set; }
        public DateTime? AdminReplyTime { get; set; }

        public virtual User User { get; set; }
    }
}
