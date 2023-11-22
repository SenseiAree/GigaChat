using System;
using System.Collections.Generic;

namespace GigaChatDALCrossPlatform.Models
{
    public partial class User
    {
        public User()
        {
            ChatInitiators = new HashSet<Chat>();
            ChatRecipients = new HashSet<Chat>();
            Feedbacks = new HashSet<Feedback>();
            FriendListFriends = new HashSet<FriendList>();
            FriendListUsers = new HashSet<FriendList>();
            MessageReceivers = new HashSet<Message>();
            MessageSenders = new HashSet<Message>();
            StarredMessages = new HashSet<StarredMessage>();
        }

        public int UserId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationTime { get; set; }

        public virtual Role Role { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Chat> ChatInitiators { get; set; }
        public virtual ICollection<Chat> ChatRecipients { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<FriendList> FriendListFriends { get; set; }
        public virtual ICollection<FriendList> FriendListUsers { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }
        public virtual ICollection<Message> MessageSenders { get; set; }
        public virtual ICollection<StarredMessage> StarredMessages { get; set; }
    }
}
