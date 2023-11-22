using GigaChatDALCrossPlatform.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace GigaChatDALCrossPlatform
{
    /// <summary>
    /// The repository class for User related operations
    /// </summary>
    public class UserRepository
    {

        private readonly GigaChatDBContext context;

        /// <summary>
        /// The constructor for the repository class of User with dependency injection
        /// </summary>
        /// <param name="gigaChatDbContext"></param>
        public UserRepository(GigaChatDBContext gigaChatDbContext)
        {
            this.context = gigaChatDbContext;
        }

        #region User
        /// <summary>
        /// Registers the User accordingly in the User Table with the required parameters      
        /// </summary>
        /// <param name="emailId"> Stores the EmailId of the User </param>
        /// <param name="password"> Stores the Password of the User </param>
        /// <param name="displayName"> Stores the Name of the User to be displayed </param>
        /// <param name="dateOfBirth"> Stores the date of birth of the user</param>
        /// <param name="userId">An out parameter to store the User Id from the stored procedure</param>
        /// <returns>
        /// New User on successful insertion,
        /// -1 If the provided Email Id already exists,
        /// -2 If the provided Email Id is invalid,
        /// -3 If the provided Password is invalid,
        /// -4 If the provided Name is invalid,
        /// -5 If the Age of the User is less than 13
        /// -99 If there is any other exception in the database
        /// -98 If there is any other exception in the code
        /// </returns>
        public User RegisterUser(string emailId, string password, string displayName, DateTime dateOfBirth, out int userId)
        {
            int result = -1;
            User newUser = new();
            int newUserId = 0;
            userId = 0;
            int rowsAffected = -1;
            //Input
            SqlParameter prmEmailId = new("@EmailId", emailId);
            SqlParameter prmPassword = new("@Password", password);
            SqlParameter prmDisplayName = new("@DisplayName", displayName);
            SqlParameter prmDOB = new("@DateOfBirth", dateOfBirth);

            //Output
            SqlParameter prmUserId = new("@UserId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
            SqlParameter prmReturn = new("@Return", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };

            try
            {
                rowsAffected = context.Database.ExecuteSqlRaw("EXEC @Return = usp_RegisterUser @EmailId, @Password, @DisplayName, @DateOfBirth, @UserId OUT",
                    prmReturn, prmEmailId, prmPassword, prmDisplayName, prmDOB, prmUserId);
                result = Convert.ToInt32(prmReturn.Value);
                if (rowsAffected > 0)
                {
                    userId = Convert.ToInt32(prmUserId.Value);
                    newUserId = userId;

                }
                if(result == 1)
                {
                    newUser = context.Users.FirstOrDefault(p => p.UserId == newUserId);
                    UserProfile userProfile = context.UserProfiles.Find(newUserId);
                    if (userProfile != null)
                    {
                        userProfile.IsActive = "Online";
                        context.SaveChanges();
                    }
                }
                else
                {
                    newUser = new() { UserId = result, RoleId = -1 };
                }

            }
            catch (Exception ex)
            {
                newUser = new() { UserId = -98, RoleId = -98 };
            }
            return newUser;
        }
        /// <summary>
        /// The Login Method to validate the credentials of the user based on the given Email Id and Password
        /// </summary>
        /// <param name="emailId"> Stores the Email Id of the User to be logged in </param>
        /// <param name="password">
        /// Stores the Password of the User to be logged in
        /// </param>
        /// <param name="userId">
        /// Out Parameter to store the UserId After the User is logged in
        /// </param>
        /// <returns>
        /// -1 If the Email Id and Password combination is invalid,
        /// 1 If the Email Id and Password combination is valid and the user is an Admin,
        /// 2 If the Email Id and Password combination is valid and the user is a User,
        /// -98 If there is any other exception in the code
        /// </returns>
        public int ValidateCredentials(string emailId, string password, out int userId)
        {
            int roleId = -1;
            userId = -1;
            try
            {
                User user = context.Users.FirstOrDefault(p => p.EmailId == emailId && p.Password == password);
                if (user != null)
                {
                    userId = user.UserId;
                    roleId = user.RoleId;
                    UserProfile userProfile = context.UserProfiles.Find(userId);
                    if (userProfile != null)
                    {
                        userProfile.IsActive = "Online";
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                roleId = -98;
            }
            return roleId;
        }
        /// <summary>
        /// Logs out the User based on the given User Id
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User to be logged out
        /// </param>
        /// <returns>
        /// True if the User is logged out successfully,
        /// False if the User is not logged out successfully.
        /// </returns>
        public bool Logout(int userId)
        {
            bool status = false;
            try
            {
                UserProfile userProfile = context.UserProfiles.FirstOrDefault(p => p.UserId == userId);
                if (userProfile != null)
                {
                    userProfile.IsActive = "Offline";
                    context.UserProfiles.Update(userProfile);
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        /// <summary>
        /// Gets the User details based on the given User Id from the User Table
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User to be retrieved
        /// </param>
        /// <returns>
        /// User object if the User Id is valid,
        /// null If the User Id is invalid
        /// </returns>
        public User GetUser(int userId)
        {
            User user = null;
            try
            {
                user = context.Users.FirstOrDefault(p => p.UserId == userId);

            }
            catch (Exception)
            {
                user = null;
            }
            return user;
        }

        /// <summary>
        /// Updates the Theme of the given User Id to the given Theme
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Theme is to be updated
        /// </param>
        /// <returns>
        /// True if the Theme is updated successfully,
        /// False if the Theme is not updated successfully.
        /// </returns>
        public bool UpdateDisplayName(int userId, string displayName)
        {
            bool status = false;
            try
            {

                User userProfile = GetUser(userId);
                if (userProfile != null)
                {
                    userProfile.DisplayName = displayName;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        public List<User> GetAllUsers()
        {
            List<User> users = null;
            try
            {
                users = context.Users.Where(user => user.RoleId == 1).ToList();
            }
            catch (Exception)
            {
                users = null;
            }
            return users;
        }
        /// <summary>
        /// Gets the List of Users the User had Chatted with
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Chat Users are to be fetched
        /// </param>
        /// <returns>
        /// List of Users the User had Chatted with,
        /// null if there is any exception in the code
        /// Empty List if the User had not Chatted with any other User
        /// </returns>
        public List<User> GetChatUsersFromUserId(int userId)
        {
            List<User> users = new();
            try
            {
                var chats = context.Chats.Where(p => p.InitiatorId == userId || p.RecipientId == userId).ToList();
                if (chats != null)
                {
                    users = new List<User>();
                    foreach (var chat in chats)
                    {
                        //Get the user object of the other user
                        var user = chat.InitiatorId == userId ?
                            context.Users.FirstOrDefault(p => p.UserId == chat.RecipientId) :
                            context.Users.FirstOrDefault(p => p.UserId == chat.InitiatorId);

                        //Add the user object to the list
                        users.Add(user);
                    }
                }
            }
            catch (Exception)
            {
                users = null;
            }
            return users;
        }

        #endregion

        #region Chat
        /// <summary>
        /// Gets the list of Chats for the given User Id from the Chat Table
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose chats are to be retrieved
        /// </param>
        /// <returns>
        /// List of Chat objects if the User Id is valid,
        /// null If the User Id is invalid
        /// </returns>
        public List<Chat> GetChats(int userId)
        {
            List<Chat> lstChat = null;
            try
            {
                if (userId != 0)
                {
                    lstChat = context.Chats
                        .Where(p => p.InitiatorId == userId || p.RecipientId == userId)
                        .OrderBy(p => p.Recipient.DisplayName).ToList();
                }

            }
            catch (Exception)
            {
                lstChat = null;
            }
            return lstChat;
        }

        /// <summary>
        /// Gets the Chat Id from the Chat Table based on the given Initiator Id and Recipient Id
        /// </summary>
        /// <param name="initiatorId">
        /// Stores the User Id of Initiator (can be recipient)   
        /// </param>
        /// <param name="recipientId">
        /// Stores the User Id of Recipient (can be initiator)
        /// </param>
        /// <returns>
        /// ChatId if the given Initiator Id and Recipient Id combination is valid,
        /// -1 If the given Initiator Id and Recipient Id combination is invalid,
        /// -98 If there is any other exception in the code
        /// </returns>
        public int GetChatId(int initiatorId, int recipientId)
        {
            int chatId = -1;

            try
            {
                var chatObj = context.Chats.Where
                    (p =>
                        (p.InitiatorId == initiatorId && p.RecipientId == recipientId) ||
                        (p.InitiatorId == recipientId && p.RecipientId == initiatorId)).FirstOrDefault();

                if (chatObj != null)
                {
                    chatId = chatObj.ChatId;
                    return chatId;
                }

            }
            catch (Exception)
            {
                chatId = -98;
            }

            return chatId;
        }

        /// <summary>
        /// Adds a new Chat to the Chat Table based on the given Initiator Id and Recipient Id if the Chat does not already exist
        /// </summary>
        /// <param name="initiatorId">
        /// Stores the User Id of Initiator (can be recipient)
        /// </param>
        /// <param name="recipientId">
        /// Stores the User Id of Recipient (can be initiator)
        /// </param>
        /// <returns>
        /// Chat Id of the newly added Chat if the Chat does not already exist,
        /// Existing Chat Id if the Chat already exists,
        /// -98 if there is any other exception in the code
        /// </returns>
        public int AddChat(int initiatorId, int recipientId)
        {
            int status;
            Chat chat = new()
            {
                InitiatorId = initiatorId,
                RecipientId = recipientId,
            };

            try
            {


                int existingChatId = GetChatId(initiatorId, recipientId);

                if (existingChatId != -1 || existingChatId == -98)
                {
                    status = existingChatId;
                }
                else
                {
                    context.Chats.Add(chat);
                    context.SaveChanges();
                    status = GetChatId(initiatorId, recipientId);

                }

            }
            catch (Exception)
            {
                status = -98;
            }

            return status;
        }

        /// <summary>
        /// Deletes the Entire Chat based on the given Chat Id, if there are no messages in it
        /// </summary>
        /// <param name="chatId">
        /// Stores the Chat Id of the Chat to be deleted
        /// </param>
        /// <param name="userId">
        /// Stores UserId of User for which chat is to be deleted
        /// </param>
        /// <returns>
        /// True if the Chat is deleted successfully,
        /// False if the Chat is not deleted successfully.
        /// </returns>
        public bool DeleteChat(int chatId, int userId)
        {
            bool status = false;
            try
            {
                List<Message> messagesInChat = context.Messages.Where(message => message.ChatId == chatId).ToList();
                Chat chat = context.Chats.FirstOrDefault(chat => chat.ChatId == chatId && (chat.InitiatorId == userId || chat.RecipientId == userId));
                List<Message> messagesInChatForUser = context.Messages.Where(message => message.ChatId == chatId && message.SenderId == userId).ToList();
                

                if (chat != null && messagesInChatForUser.Count > 0)
                {
                    foreach(Message message in messagesInChatForUser)
                    {
                        if(message.SenderId == userId)
                        {
                            DeleteMessage(message.MessageId, message.SenderId);
                            
                        }
                    }
                    
                    messagesInChat = context.Messages.Where(message => message.ChatId == chatId).ToList();
                    status = true;

                }
                
                if (chat != null && messagesInChat.Count == 0)
                {
                    context.Chats.Remove(chat);
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        #endregion

        #region FriendList
        /// <summary>
        /// Adds a new Friend to the FriendList Table
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User who is adding the Friend
        /// </param>
        /// <param name="friendId">
        /// Stores the User Id of the Friend to be added
        /// </param>
        /// <returns>
        /// True if the Friend is added successfully,
        /// False if the Friend is not added successfully.
        /// </returns>
        public bool AddFriend(int userId, int friendId)
        {
            bool status = false;
            FriendList friendList = new()
            {
                UserId = userId,
                FriendId = friendId
            };
            try
            {
                context.FriendLists.Add(friendList);
                context.SaveChanges();
                status = true;

            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }
        // FriendList with users who are not blocked

        /// <summary>
        /// Gets the FriendList of the given User Id who are not blocked
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose FriendList is to be retrieved
        /// </param>
        /// <returns>
        /// List of FriendList of the given User Id who are not blocked if the User Id is valid,
        /// null if the User Id is invalid or if there is any other exception in the code
        /// </returns>
        public List<FriendList> GetFriendList(int userId)
        {
            List<FriendList> friendList = new();

            try
            {
                //friendList = gigaChatDbContext.FriendLists.Where(p => p.UserId == userId && !p.IsBlocked).ToList();
                friendList = context.FriendLists.Where(p => p.UserId == userId).ToList();
            }
            catch (Exception)
            {
                friendList = null;
            }
            return friendList;
        }

        /// <summary>
        /// Blocks the given Friend of the given User Id
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User who is blocking the Friend
        /// </param>
        /// <param name="friendId">
        /// Stores the User Id of the Friend to be blocked
        /// </param>
        /// <returns>
        /// True if the Friend is blocked successfully,
        /// False if the Friend is not blocked successfully.
        /// </returns>
        public bool BlockUser(int userId, int friendId)
        {
            bool status = false;
            try
            {
                FriendList friendList = context.FriendLists
                    .Where(p => p.UserId == userId && p.FriendId == friendId && !p.IsBlocked).FirstOrDefault();
                if (friendList != null)
                {
                    friendList.IsBlocked = true;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {

                status = false; ;
            }
            return status;
        }

        /// <summary>
        /// Unblocks the given Friend of the given User Id
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User who is unblocking the Friend
        /// </param>
        /// <param name="friendId">
        /// Stores the User Id of the Friend to be unblocked
        /// </param>
        /// <returns>
        /// True if the Friend is unblocked successfully,
        /// False if the Friend is not unblocked successfully.
        /// </returns>
        public bool UnblockUser(int userId, int friendId)
        {
            bool status = false;
            try
            {
                FriendList friendList = context.FriendLists
                    .Where(p => p.UserId == userId && p.FriendId == friendId && p.IsBlocked).FirstOrDefault();
                if (friendList != null)
                {
                    friendList.IsBlocked = false;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }

        /// <summary>
        /// Gets the FriendList of the given User Id who are blocked
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose blocked FriendList is to be retrieved
        /// </param>
        /// <returns>
        /// List of FriendList of the given User Id who are blocked if the User Id is valid,
        /// null if the User Id is invalid or if there is any other exception in the code
        /// </returns>
        public List<FriendList> GetBlockedFriendList(int userId)
        {
            List<FriendList> friendList = null;
            try
            {
                friendList = context.FriendLists.Where(p => p.IsBlocked && p.UserId == userId).ToList();
            }
            catch (Exception)
            {

                friendList = null;
            }
            return friendList;
        }
        /// <summary>
        /// Removes Friend with given friendId from FriendList of User with given userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns>
        /// True, if removal of Friend is successfull
        /// False, if removal of Friend is unsuccessfull or if an error has occured
        /// </returns>
        public bool DeleteFriend(int userId, int friendId)
        {
            bool status = false;
            try
            {
                FriendList friendList = context.FriendLists
                    .FirstOrDefault(friend => friend.UserId == userId && friend.FriendId == friendId);
                if (friendList != null)
                {
                    context.FriendLists.Remove(friendList);
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        #endregion

        #region Feedback

        /// <summary>
        /// Adds the Feedback of the User to the Feedback Table
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User who is giving the Feedback
        /// </param>
        /// <param name="userFeedback">
        /// Stores the Feedback given by the User
        /// </param>
        /// <param name="rating">
        /// Stores the Rating given by the User. Should Range from 1 to 5
        /// </param>
        /// <returns>
        /// True if the Feedback is added successfully,
        /// False if the Feedback is not added successfully.
        /// </returns>
        public bool AddFeedback(int userId, string userFeedback, int rating)
        {
            bool status = false;

            Feedback feedback = new()
            {
                UserId = userId,
                UserFeedback = userFeedback,
                Rating = rating
            };

            try
            {
                context.Feedbacks.Add(feedback);
                context.SaveChanges();
                status = true;
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        /// <summary>
        /// Gets the Feedback of the given User Id
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Feedback is to be retrieved
        /// </param>
        /// <returns>
        /// List of Feedback of the given User Id if the User Id is valid,
        /// null if the User Id is invalid or if there is any other exception in the code
        /// </returns>
        public List<Feedback> GetFeedback(int userId)
        {
            List<Feedback> lstFeedback = new();

            try
            {
                lstFeedback = context.Feedbacks.Where(p => p.UserId == userId).ToList();
            }
            catch (Exception)
            {

                lstFeedback = null;
            }
            return lstFeedback;
        }


        #endregion

        #region UserProfile 
        /// <summary>
        /// Gets the User Profile of the given User Id
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose User Profile is to be retrieved
        /// </param>
        /// <returns>
        /// Object of UserProfile of the given User Id if the User Id is valid,
        /// null if the User Id is invalid or if there is any other exception in the code
        /// </returns>
        public UserProfile GetUserProfile(int userId)
        {
            UserProfile userProfile = new();

            try
            {
                userProfile = context.UserProfiles.Where(p => p.UserId == userId).FirstOrDefault();

            }
            catch (Exception)
            {

                userProfile = null;
            }

            return userProfile;
        }

        /// <summary>
        /// Updates the Availability Status of the given User Id to the given Availability Status
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Availability Status is to be updated
        /// </param>
        /// <param name="newAvailabilityStatus">
        /// Stores the new Availability Status of the User
        /// </param>
        /// <returns>
        /// True if the Availability Status is updated successfully,
        /// False if the Availability Status is not updated successfully.
        /// </returns>
        public bool UpdateAvailabilityStatus(int userId, string newAvailabilityStatus)
        {
            bool status = false;

            try
            {

                UserProfile userProfile = GetUserProfile(userId);
                if (userProfile != null)
                {
                    userProfile.AvailabilityStatus = newAvailabilityStatus;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;

        }

        /// <summary>
        /// Gets the User Profile of the given Email Id
        /// </summary>
        /// <param name="emailId">
        /// Stores the Email Id of the User whose User Profile is to be retrieved
        /// </param>
        /// <returns>
        /// Object of UserProfile of the given Email Id if the Email Id is valid,
        /// null if the Email Id is invalid or if there is any other exception in the code
        /// </returns>
        public UserProfile GetUserProfileFromEmailId(string emailId)
        {
            UserProfile userProfile;

            try
            {
                userProfile = context.UserProfiles.Where(p => p.User.EmailId == emailId).FirstOrDefault();
            }
            catch (Exception)
            {

                userProfile = null;
            }

            return userProfile;
        }



        /// <summary>
        /// Updates the Avatar of the given User Id to the given Avatar
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Avatar is to be updated
        /// </param>
        /// <param name="newAvatar">
        /// Stores the new Avatar of the User
        /// </param>
        /// <returns>
        /// True if the Avatar is updated successfully,
        /// False if the Avatar is not updated successfully.
        /// </returns>
        public bool UpdateAvatar(int userId, string newAvatar)
        {
            bool status = false;
            try
            {

                UserProfile userProfile = GetUserProfile(userId);
                if (userProfile != null)
                {
                    userProfile.Avatar = newAvatar;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        /// <summary>
        /// Toggles the Theme of a User based on the User Id from "Light" to "Dark" and vice-versa
        /// </summary>
        /// <param name="userId">
        /// Stores the Id of the User whose Theme is Toggled
        /// </param>
        /// <returns>
        /// True if the Theme is toggled successfully,
        /// False if there is any expection
        /// </returns>
        public string ToggleTheme(int userId)
        {
            string status = null;
            try
            {
                UserProfile temp = context.UserProfiles.FirstOrDefault(userProfile => userProfile.UserId == userId);
                if (temp != null)
                {
                    temp.Theme = temp.Theme == "Light" ? "Dark" : "Light";
                    context.UserProfiles.Update(temp);
                    context.SaveChanges();
                    status = temp.Theme;
                }
            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();
            }
            return status;
        }

        public List<UserProfile> GetAllUserProfiles()
        {
            List<UserProfile> users = null;
            try
            {
                users = context.UserProfiles.Where(userProfile => userProfile.User.RoleId == 1).ToList();
            }
            catch (Exception)
            {
                users = null;
            }
            return users;
        }

        /// <summary>
        /// Gets the List of User Profiles the User had Chatted with
        /// </summary>
        /// <param name="userId">
        /// Stores the User Id of the User whose Chat User Profiles are to be fetched
        /// </param>
        /// <returns>
        /// List of User Profiles the User had Chatted with,
        /// Empty List if the User had not Chatted with any other User,
        /// null if there is any exception in the code
        /// </returns>
        public List<UserProfile> GetChatUserProfilesFromUserId(int userId)
        {
            List<UserProfile> userProfiles = new();
            try
            {
                var chats = context.Chats.Where(p => p.InitiatorId == userId || p.RecipientId == userId).ToList();
                if (chats != null)
                {
                    userProfiles = new List<UserProfile>();
                    foreach (var chat in chats)
                    {
                        //Get the user object of the other user
                        var userProfile = chat.InitiatorId == userId ?
                            context.UserProfiles.FirstOrDefault(p => p.UserId == chat.RecipientId) :
                            context.UserProfiles.FirstOrDefault(p => p.UserId == chat.InitiatorId);

                        //Add the user object to the list
                        userProfiles.Add(userProfile);
                    }
                }
            }
            catch (Exception)
            {
                userProfiles = null;
            }
            return userProfiles;
        }

        #endregion

        #region Message
        /// <summary>
        /// Gets the List of Messages between the given Sender Id and Receiver Id from the Message Table
        /// </summary>
        /// <param name="senderId">
        /// Stores the User Id of Sender whose Messages are to be retrieved
        /// </param>
        /// <param name="receiverId">
        /// Stores the User Id of Receiver whose Messages are to be retrieved
        /// </param>
        /// <returns>
        /// List of Messages between the given Sender Id and Receiver Id if the Sender Id and Receiver Id are valid,
        /// null if the Sender Id and Receiver Id are invalid or if there is any other exception in the code
        /// </returns>
        public List<Message> GetMessages(int senderId, int receiverId)
        {
            List<Message> messages = null;
            int chatId = GetChatId(senderId, receiverId);
            try
            {
                if (chatId != -1 || chatId != -98)
                {
                    messages = context.Messages.Where(p => p.Chat.ChatId == chatId).Include(c => c.Chat).OrderBy(c => c.SentTime).ToList();
                }
                else
                {
                    messages = null;
                }


            }
            catch (Exception ex)
            {

                messages = null;
            }
            return messages;
        }

        /// <summary>
        /// Gets the List of Messages from the given Chat Id from the Message Table
        /// </summary>
        /// <param name="chatId">
        /// Stores the Chat Id of the Chat whose Messages are to be retrieved
        /// </param>
        /// <returns>
        /// List of Messages from the given Chat Id if the Chat Id is valid,
        /// null if the Chat Id is invalid or if there is any other exception in the code
        /// </returns>
        public List<Message> GetMessagesFromChatId(int chatId)
        {
            List<Message> messages;
            try
            {
                messages = context.Messages.Where(p => p.ChatId == chatId).OrderBy(c => c.SentTime).ToList();
                foreach (var item in messages)
                {
                    item.IsRead = "Delivered";
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                messages = null;
            }
            return messages;
        }

        /// <summary>
        /// Gets the List of Messages from the given Chat Id from the Message Table
        /// </summary>
        /// <param name="chatId">
        /// Stores the Chat Id of the Chat whose Messages are to be retrieved
        /// </param>
        /// <returns>
        /// List of Messages from the given Chat Id if the Chat Id is valid,
        /// null if the Chat Id is invalid or if there is any other exception in the code
        /// </returns>
        public List<Message> GetLatestMessages(int chatId)
        {
            List<Message> messages;
            try
            {
                messages = context.Messages.Where(p => p.Chat.ChatId == chatId && p.IsRead == "Sent").OrderBy(c => c.SentTime).ToList();
                foreach (var item in messages)
                {
                    item.IsRead = "Delivered";
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                messages = null;
            }
            return messages;
        }
        /// <summary>
        /// Returns list of unread messages
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// List of unread messages
        /// </returns>
        public List<MessageCount> GetUnreadMessages(int userId)
        {
            List<MessageCount> messages;
            try
            {
                messages = context.Messages
                   .Where(p => p.ReceiverId == userId && p.IsRead == "Sent")
                   .GroupBy(p => p.SenderId)
                   .Select(p => new MessageCount
                   {
                       SenderId = p.Key,
                       Count = p.Count()
                   })
                   .ToList();
                //foreach (var item in messages)
                //{
                //    item.IsRead = "Delivered";
                //}
                //dbContext.SaveChanges();
                //return messages;
            }
            catch (Exception ex)
            {
                messages = null;
            }
            return messages;
        }

        /// <summary>
        /// Sends the Message from the given Sender Id to the given Receiver Id
        /// </summary>
        /// <param name="senderId">
        /// Stores the User Id of the Sender who sends the Message
        /// </param>
        /// <param name="receiverId">
        /// Stores the User Id of the Receiver who receives the Message
        /// </param>
        /// <param name="messageContent">
        /// Stores the Content of the Message to be sent
        /// </param>
        /// <returns>
        /// MessageId of the Message if it is sent successfully,
        /// 0 if the Message is not sent successfully,
        /// -98 if there is any other exception in the code
        /// </returns>
        public int SendMessage(int senderId, int receiverId, string messageContent)
        {
            int status = 0;
            try
            {
                int chatId = GetChatId(senderId, receiverId);
                if (chatId != -1 || chatId != -98)
                {
                    Message newMessage = new()
                    {
                        ChatId = chatId,
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = messageContent
                    };

                    context.Messages.Add(newMessage);
                    context.SaveChanges();
                    //Commented by Areetra. Need the function to return the newly generated MessageId
                    //status = 1;  
                    status = context.Messages
                        .Where(message =>
                            message.ChatId == newMessage.ChatId &&
                            message.Content == newMessage.Content &&
                            message.SenderId == newMessage.SenderId &&
                            message.ReceiverId == newMessage.ReceiverId
                        ).OrderBy(message => message.SentTime).Last().MessageId;
                }
            }
            catch (Exception)
            {

                status = -98;
            }
            return status;

        }
        
        /// <summary>
        /// Deletes the Message from the given Message Id
        /// </summary>
        /// <param name="messageId">
        /// Stores the Message Id of the Message to be deleted
        /// </param>
        /// <returns>
        /// True if the Message is deleted successfully,
        /// False if the Message is not deleted successfully
        /// </returns>
        public int DeleteMessage(int messageId, int userId)
        {

            int status = -1;
            try
            {

                Message message = context.Messages.FirstOrDefault(p => p.MessageId == messageId && p.SenderId == userId);

                if (message != null)
                {
                    StarredMessage starredMessage = context.StarredMessages.FirstOrDefault(p => p.MessageId == messageId && p.UserId == userId);
                    if (starredMessage != null)
                    {
                        context.StarredMessages.Remove(starredMessage);
                    }
                    context.Messages.Remove(message);
                    context.SaveChanges();
                    status = 1;

                }
            }
            catch (Exception)
            {

                status = -98;
            }
            return status;
        }
        /// <summary>
        /// Editing message based on messageId and message content 
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int EditMessage(int messageId, string content)
        {
            int status = -1;
            try
            {
                Message messageToBeUpdated = context.Messages.Find(messageId);
                if (messageToBeUpdated != null)
                {
                    messageToBeUpdated.Content = content;
                    context.SaveChanges();
                    status = 1;
                }

            }
            catch (Exception)
            {
                status = -98;
            }
            return status;
        }

        #endregion

        #region StarredMessages
        /// <summary>
        /// Returns List of Starred message based on userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<StarredMessage> GetStarredMessages(int userId)
        {
            List<StarredMessage> starredMessages = null;
            try
            {
                starredMessages = context.StarredMessages.Where(starredMessage => starredMessage.UserId == userId).ToList();
            }
            catch (Exception)
            {
                starredMessages = null;
            }
            return starredMessages;
        }
        /// <summary>
        /// Deletes the Message from the given Message Id
        /// </summary>
        /// <param name="messageId">
        /// Stores the Message Id of the Message to be deleted
        /// </param>
        /// <param name="userId">
        /// Stores the User Id of the User who deletes the Message
        /// </param>
        /// <returns>
        /// True if the Message is deleted successfully,
        /// False if the Message is not deleted successfully
        /// </returns>
        public bool DeleteStarredMessage(int messageId, int userId)
        {
            bool status = false;
            try
            {
                StarredMessage starredMessage = new()
                {
                    MessageId = messageId,
                    UserId = userId
                };

                context.StarredMessages.Remove(starredMessage);
                context.SaveChanges();
                status = true;

            }
            catch (Exception)
            {

                status = false;
            }
            return status;

        }

        /// <summary>
        /// Stars the Message from the given Message Id
        /// </summary>
        /// <param name="messageId">
        /// Stores the Message Id of the Message to be starred
        /// </param>
        /// <param name="userId">
        /// Stores the User Id of the User who stars the Message
        /// </param>
        /// <returns>
        /// 1 if the Message is starred successfully,
        /// -1 if the Message is not present in the Message Table,
        /// -98 if the Message is not starred successfully.
        /// </returns>
        public int ToggleStarMessage(int messageId, int userId)
        {
            int status = -1;
            try
            {
                Message message = context.Messages.FirstOrDefault(p => p.MessageId == messageId);

                StarredMessage starredMessage = context.StarredMessages.FirstOrDefault(p => p.MessageId == messageId && p.UserId == userId);

                if (message != null)
                {
                    if (starredMessage is null)
                    {
                        StarredMessage starredMessageObj = new() { MessageId = messageId, UserId = userId };

                        context.StarredMessages.Add(starredMessageObj);
                        context.SaveChanges();
                        status = 1;
                    }
                    else
                    {
                        context.StarredMessages.Remove(starredMessage);
                        context.SaveChanges();
                        status = 0;
                    }
                }
            }
            catch (Exception)
            {

                status = -98;
            }
            return status;
        }
        #endregion
        /// <summary>
        /// Returns Avg Feedback
        /// </summary>
        /// <returns></returns>

        public double AvgFeedbackRating()
        {
            double avgFeedbackRating = 0;
            try
            {
                List<int> rating = context.Feedbacks.Select(p => p.Rating).ToList();
                if(rating.Count > 0)
                {
                    avgFeedbackRating = rating.Average();
                }
                else
                {
                    avgFeedbackRating = -1;
                }
                
            }
            catch (Exception ex)
            {

                avgFeedbackRating = -98;
            }
            return avgFeedbackRating;
            

        }/// <summary>
        /// To fetch Feedbacks with rating > 4 randomly feedbacks 
        /// </summary>
        /// <returns>
        /// List of Feedbacks, in case of successfull execution
        /// null, if failed or in case of an error
        /// </returns>
        public List<Feedback> GetTopFeedbacks()
        {
            List<Feedback> feedbacks = new();

            try
            {
                List<Feedback> feedbacksList = context.Feedbacks.Where(p=> p.Rating >= 4).ToList();

                if(feedbacksList.Count > 0)
                {
                    if(feedbacksList.Count == 1) 
                    {
                        feedbacks = feedbacksList;
                    }
                    if(feedbacksList.Count > 1)
                    {
                        Random random = new Random();

                        HashSet<int> randomIndices = new ();
                        while(randomIndices.Count < 2) 
                        {
                            randomIndices.Add(random.Next(feedbacksList.Count));
                        }

                        foreach(int i in randomIndices)
                        {
                            feedbacks.Add(feedbacksList[i]);
                        }

                    }

                }

            }
            catch (Exception)
            {

                feedbacks = null;
            }
            return feedbacks;
        }
        /// <summary>
        /// To Fetch all Feedbacks
        /// </summary>
        /// <returns>
        /// If fetching is successfull ,List of Feedbacks returned, 
        /// else, returns null
        /// </returns>
        public List<Feedback> GetAllFeedbacks()
        {
            List<Feedback> feedbacks = new();
            try
            {
                feedbacks = context.Feedbacks.ToList();
            }
            catch (Exception)
            {

                feedbacks = null;
            }
            return feedbacks;
        }

        

    }
}