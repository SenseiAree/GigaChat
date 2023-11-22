using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class FriendList
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public bool IsBlocked { get; set; }

        public virtual User Friend { get; set; }
        public virtual User User { get; set; }
    }
}
