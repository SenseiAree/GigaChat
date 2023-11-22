using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class UserProfile
    {
        public int UserId { get; set; }
        public string Avatar { get; set; }
        public string IsActive { get; set; }
        public string Theme { get; set; }
        public string AvailabilityStatus { get; set; }

        public virtual User User { get; set; }
    }
}
