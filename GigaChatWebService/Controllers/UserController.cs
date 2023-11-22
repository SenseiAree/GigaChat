using GigaChatDALCrossPlatform;
using GigaChatDALCrossPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace GigaChatWebService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        #region Constructor
        UserRepository repository;
        public UserController(UserRepository userRepository)
        {
            this.repository = userRepository;
        }

        #endregion

        [HttpGet]
        public string ServerStatus()
        {
            return "Server is running";
        }

        #region Registartion and Authentication    
        /// <summary>
        /// Service Method for Registering newUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// User, if registeration is successfull
        /// User, with RoleId = -1, and UserId = -1, if registeration faild
        /// User, with RoleId = -98 ad UserId = -98, in case of error
        /// </returns>
        [HttpPost]
        public JsonResult RegisterUser(Models.User user)
        {
            Models.User newUser = null;
            int userId = -1;
            try
            {
                User userToBeFetched = repository.RegisterUser(user.EmailId, user.Password, user.DisplayName, user.DateOfBirth, out userId);
                newUser = new()
                {
                    EmailId = userToBeFetched.EmailId,
                    Password = userToBeFetched.Password,
                    DisplayName = userToBeFetched.DisplayName,
                    DateOfBirth = userToBeFetched.DateOfBirth,
                    UserId = userToBeFetched.UserId,
                    RoleId = userToBeFetched.RoleId,
                };
            }
            catch (Exception)
            {
                newUser =new() {UserId = -97, RoleId = -98 };
            }
            return Json(newUser);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateCredentials(string emailId, string password)
        {
            Models.User user;
            int roleId, userId;
            try
            {
                roleId = repository.ValidateCredentials(emailId, password, out userId);
                user = new() { RoleId = roleId, UserId = userId };
            }
            catch (Exception)
            {
                user = null;
            }
            return Json(user);
        }

        #endregion

        #region User
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUser(int userId)
        {
            Models.User user = null;
            try
            {
                User temp = repository.GetUser(userId);
                if (temp != null)
                {
                    user = new Models.User()
                    {
                        UserId = temp.UserId,
                        EmailId = temp.EmailId,
                        Password = temp.Password,
                        RoleId = temp.RoleId,
                        DisplayName = temp.DisplayName,
                        DateOfBirth = temp.DateOfBirth
                    };
                }
            }
            catch (Exception)
            {
                user = null;
            }
            return Json(user);
        }

        #endregion

        #region Chat

        [HttpGet]
        public JsonResult GetChats(int userId)
        {
            List<Models.Chat> lstChat = null;
            try
            {
                lstChat = repository.GetChats(userId).Select(c => new Models.Chat()
                {
                    ChatId = c.ChatId,
                    InitiatorId = c.InitiatorId,
                    RecipientId = c.RecipientId
                }).ToList();
            }
            catch (Exception)
            {
                lstChat = null;
            }
            return Json(lstChat);
        }

        [HttpGet]
        public int GetChatId(int initiatorId, int recipientId)
        {
            int chatId = -1;

            try
            {
                chatId = repository.GetChatId(initiatorId, recipientId);
            }
            catch (Exception)
            {
                chatId = -97;
            }

            return chatId;
        }

        [HttpPost]
        public int AddChat(Models.Chat chat)
        {
            int status = -1;
            try
            {
                status = repository.AddChat(chat.InitiatorId, chat.RecipientId);
            }
            catch (Exception)
            {
                status = -97;
            }

            return status;
        }

        [HttpDelete]
        public bool DeleteChat(int chatId, int userId)
        {
            bool status = false;
            try
            {
                status = repository.DeleteChat(chatId,userId);
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        [HttpGet]
        public JsonResult GetChatUsersFromUserId(int userId)
        {
            List<Models.User> users;
            try
            {
                users = repository.GetChatUsersFromUserId(userId).Select(u => new Models.User()
                {
                    UserId = u.UserId,
                    DisplayName = u.DisplayName,
                    EmailId = u.EmailId,
                    DateOfBirth = u.DateOfBirth,
                    RoleId = u.RoleId
                }).ToList();
            }
            catch (Exception)
            {
                users = null;
            }
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetChatUserProfilesFromUserId(int userId)
        {
            List<Models.UserProfile> userProfiles;
            try
            {
                userProfiles = repository.GetChatUserProfilesFromUserId(userId).Select(u => new Models.UserProfile()
                {
                    UserId = u.UserId,
                    AvailabilityStatus = u.AvailabilityStatus,
                    Avatar = u.Avatar,
                    IsActive = u.IsActive
                }).ToList();
            }
            catch (Exception)
            {
                userProfiles = null;
            }
            return Json(userProfiles);
        }

        [HttpGet]
        public JsonResult GetUnreadMessages(int userId)
        {
            List<MessageCount> messages;
            try
            {
                messages = repository.GetUnreadMessages(userId);
            }
            catch (Exception)
            {

                throw;
            }
            return Json(messages);
        }
        #endregion

        #region FriendList

        [HttpPost]
        public bool AddFriend(Models.FriendList friend)
        {
            bool status = false;
            try
            {
                status = repository.AddFriend(friend.UserId, friend.FriendId);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpDelete]
        public bool DeleteFriend(Models.FriendList friend)
        {
            bool status = false;
            try
            {
                status = repository.DeleteFriend(friend.UserId, friend.FriendId);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpGet]
        public List<Models.FriendList> GetFriendList(int userId)
        {
            List<Models.FriendList> friendList = null;
            try
            {
                List<FriendList> temp = repository.GetFriendList(userId);
                if (temp != null)
                {
                    friendList = temp.Select(f => new Models.FriendList()
                    {
                        UserId = f.UserId,
                        FriendId = f.FriendId,
                        IsBlocked = f.IsBlocked
                    }).ToList();
                }
            }
            catch (Exception)
            {
                friendList = null;
            }
            return friendList;
        }

        [HttpPut]
        public bool BlockUser(Models.FriendList blockFriend)
        {
            bool status = false;
            try
            {
                status = repository.BlockUser(blockFriend.UserId, blockFriend.FriendId);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpPut]
        public bool UnblockUser(Models.FriendList unblockFriend)
        {
            bool status = false;
            try
            {
                status = repository.UnblockUser(unblockFriend.UserId, unblockFriend.FriendId);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpGet]
        public List<Models.FriendList> GetBlockedFriendList(int userId)
        {
            List<Models.FriendList> friendList = null;
            try
            {
                friendList = repository.GetBlockedFriendList(userId).Select(f => new Models.FriendList()
                {
                    UserId = f.UserId,
                    FriendId = f.FriendId,
                    IsBlocked = f.IsBlocked
                }).ToList();
            }
            catch (Exception)
            {
                friendList = null;
            }
            return friendList;
        }

        #endregion

        #region Feedback

        [HttpPost]
        public bool AddFeedback(Models.Feedback feedback)
        {
            bool status = false;
            try
            {
                status = repository.AddFeedback(feedback.UserId, feedback.UserFeedback, feedback.Rating);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpGet]
        public List<Models.Feedback> GetFeedback(int userId)
        {
            List<Models.Feedback> lstFeedback = new List<Models.Feedback>();

            try
            {
                lstFeedback = repository.GetFeedback(userId).Select(f => new Models.Feedback()
                {
                    UserId = f.UserId,
                    UserFeedback = f.UserFeedback,
                    Rating = f.Rating,
                    PostedAt = f.PostedAt,
                    AdminReply = f.AdminReply,
                    AdminReplyTime = f.AdminReplyTime
                }).ToList();
            }
            catch (Exception)
            {
                lstFeedback = null;
            }
            return lstFeedback;
        }

        #endregion

        #region UserProfile

        [HttpGet]
        public Models.UserProfile GetUserProfile(int userId)
        {
            Models.UserProfile userProfile = null;

            try
            {
                var tempUserProfile = repository.GetUserProfile(userId);
                if (tempUserProfile != null)
                {
                    userProfile = new Models.UserProfile()
                    {
                        UserId = tempUserProfile.UserId,
                        IsActive = tempUserProfile.IsActive,
                        AvailabilityStatus = tempUserProfile.AvailabilityStatus,
                        Avatar = tempUserProfile.Avatar,
                        Theme = tempUserProfile.Theme
                    };
                }
            }
            catch (Exception)
            {
                userProfile = null;
            }
            return userProfile;
        }

        [HttpPut]
        public bool UpdateAvailabilityStatus(int userId, string newAvailabilityStatus)
        {
            bool status = false;

            try
            {
                status = repository.UpdateAvailabilityStatus(userId, newAvailabilityStatus);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpPut]
        public bool UpdateDisplayName(int userId,string displayName)
        {
            bool status = false;
            try
            {
                status = repository.UpdateDisplayName(userId,displayName);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        [HttpPut]
        public bool UpdateAvatar(int userId, string newAvatar)
        {
            bool status = false;
            try
            {
                status = repository.UpdateAvatar(userId, newAvatar);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        #endregion

        [HttpGet]
        public JsonResult GetMessagesFromChatId(int chatId)
        {
            List<Models.Message> messages = null;
            try
            {
                var temp = repository.GetMessagesFromChatId(chatId);
                if (temp != null)
                {
                    messages = temp.Select(m => new Models.Message()
                    {
                        MessageId = m.MessageId,
                        ChatId = m.ChatId,
                        SenderId = m.SenderId,
                        Content = m.Content,
                        IsRead = m.IsRead,
                        ReceiverId = m.ReceiverId,
                        SentTime = m.SentTime
                    }).ToList();
                }
            }
            catch (Exception)
            {
                messages = null;
                throw;
            }
            return Json(messages);
        }

        [HttpPut]
        public bool Logout(int userId)
        {
            bool status;
            try
            {
                status = repository.Logout(userId);
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }


        //Create a Post method to send a message using Object Mapping
        /// <summary>
        /// Post Method to send a message
        /// </summary>
        /// <param name="message">
        /// Stores the message details
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        public int SendMessage(Models.Message message)
        {
            int status = 0;
            try
            {
                status = repository.SendMessage(message.SenderId, message.ReceiverId, message.Content);
            }
            catch (Exception)
            {
                status = -97;
            }
            return status;
        }

        [HttpPut]
        public JsonResult ToggleTheme(Models.User user)
        {
            string status;
            try
            {
                status = repository.ToggleTheme(user.UserId);
            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();
            }
            return Json(status);
        }

        [HttpGet]
        public JsonResult GetAllUsers()
        {
            List<Models.User> users = null;
            try
            {
                List<User> temp = repository.GetAllUsers();
                if (temp != null)
                {
                    users = temp.Select(user => new Models.User()
                    {
                        UserId = user.UserId,
                        DateOfBirth = user.DateOfBirth,
                        DisplayName = user.DisplayName,
                        EmailId = user.EmailId,
                        Password = user.Password,
                        RoleId = user.RoleId
                    }).ToList();
                }
            }
            catch (Exception)
            {
                users = null;
            }
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetAllUserProfiles()
        {
            List<Models.UserProfile> userProfiles = null;
            try
            {
                List<UserProfile> temp = repository.GetAllUserProfiles();
                if (temp != null)
                {
                    userProfiles = temp.Select(
                        userProfile => new Models.UserProfile()
                        {
                            UserId = userProfile.UserId,
                            AvailabilityStatus = userProfile.AvailabilityStatus,
                            IsActive = userProfile.IsActive,
                            Avatar = userProfile.Avatar,
                            Theme = userProfile.Theme
                        }).ToList();
                }
            }
            catch (Exception)
            {
                userProfiles = null;
            }
            return Json(userProfiles);
        }

        [HttpDelete]
        public int DeleteMessage(int messageId, int userId)
        {
            int status = -2;
            try
            {
                status = repository.DeleteMessage(messageId, userId);
            }
            catch (Exception)
            {
                status = -97;
            }
            return status;
        }

        [HttpGet]
        public int ToggleStarMessage(int messageId, int userId)
        {
            int status = -2;
            try
            {
                status = repository.ToggleStarMessage(messageId, userId);
            }
            catch (Exception)
            {
                status = -97;
            }
            return status;
        }

        [HttpGet]
        public List<Models.StarredMessage> GetStarredMessages(int userId)
        {
            List<Models.StarredMessage> starredMessages = null;
            try
            {
                List<StarredMessage> temp = repository.GetStarredMessages(userId);
                if (temp != null)
                {
                    starredMessages = temp.Select(
                                               starredMessage => new Models.StarredMessage()
                                               {
                                                   MessageId = starredMessage.MessageId,
                                                   StarredMessageId = starredMessage.StarredMessageId,
                                                   UserId = starredMessage.UserId
                                               }).ToList();
                }
            }
            catch (Exception)
            {
                starredMessages = null;
            }
            return starredMessages;
        }

        [HttpPut]
        public int EditMessage(Models.Message message)
        {
            int status = -2;
            try
            {                
                status = repository.EditMessage(message.MessageId,message.Content);
            }
            catch (Exception)
            {
                status = -97;
            }
            return status;
        }

        [HttpGet]
        public List<Models.Feedback> GetAllFeedbacks()
        {
            List<Models.Feedback> feedbacks = null;
            try
            {
                feedbacks = repository.GetAllFeedbacks().Select(feedback => new Models.Feedback()
                {
                    FeedbackId = feedback.FeedbackId,
                    UserId = feedback.UserId,
                    UserFeedback = feedback.UserFeedback,
                    Rating = feedback.Rating,
                    PostedAt = feedback.PostedAt,
                    AdminReply = feedback.AdminReply,
                    AdminReplyTime = feedback.AdminReplyTime
                }).ToList();
            }
            catch (Exception)
            {
                feedbacks = null;
            }
            return feedbacks;
        }

        [HttpGet]
        public JsonResult GetTopFeedbacks() { 
            List<Models.Feedback> feedbacks = null;
            try
            {
                feedbacks = repository.GetTopFeedbacks().Select(feedback => new Models.Feedback()
                {
                    FeedbackId = feedback.FeedbackId,
                    UserId = feedback.UserId,
                    UserFeedback = feedback.UserFeedback,
                    Rating = feedback.Rating,
                    PostedAt = feedback.PostedAt,
                    AdminReply = feedback.AdminReply,
                    AdminReplyTime = feedback.AdminReplyTime
                }).ToList();
            }
            catch (Exception)
            {
                feedbacks = null;
            }
            return Json(feedbacks);
        }
    }
    
}
