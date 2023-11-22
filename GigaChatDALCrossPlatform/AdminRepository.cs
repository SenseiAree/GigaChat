using GigaChatDALCrossPlatform.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace GigaChatDALCrossPlatform
{
    public class AdminRepository
    {
        GigaChatDBContext dbContext;

        /// <summary>
        /// The constructor for the repository class of Admin with dependency injection
        /// </summary>
        /// <param name="gigaChatDbContext">
        /// Stores the instance of the database context of GigaChat Database
        /// </param>
        public AdminRepository(GigaChatDBContext gigaChatDbContext)
        {
            this.dbContext = gigaChatDbContext;
        }

        /// <summary>
        /// Gets the Average Rating of the application
        /// </summary>
        /// <returns>
        /// Value of the Average Rating of the application,
        /// -98 if any exception occurs.
        /// </returns>
        public double GetAverageRating()
        {
            double averageRating = -1;
            try
            {
                List<Feedback> feedbacks = dbContext.Feedbacks.ToList();
                if (feedbacks != null && feedbacks.Count > 0)
                {
                    averageRating = feedbacks.Average(p => p.Rating);
                }
                else
                {
                    averageRating = 0;
                }
            }
            catch (Exception)
            {

                averageRating = -98;
            }
            return averageRating;
        }

        /// <summary>
        /// Gets the Users who has given feedback
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllFeedbackUsers()
        {
            List<User> users = new();
            try
            {
                List<int> feedbacksUserId = dbContext.Feedbacks.Select(p => p.UserId).ToList();
                if (feedbacksUserId != null && feedbacksUserId.Count > 0)
                {
                    foreach (int userId in feedbacksUserId)
                    {
                        User userObj = dbContext.Users.Find(userId);
                        users.Add(userObj);
                    }
                }
            }
            catch (Exception)
            {

                users = null;
            }
            return users;
        }

        /// <summary>
        /// Gets the UserProfiles of the users who has given feedback
        /// </summary>
        /// <returns></returns>
        public List<UserProfile> GetAllFeedbackUserProfiles()
        {
            List<UserProfile> userProfiles = new();
            try
            {
                List<int> feedbacksUserId = dbContext.Feedbacks.Select(p => p.UserId).ToList();
                if (feedbacksUserId != null && feedbacksUserId.Count > 0)
                {
                    foreach (int userId in feedbacksUserId)
                    {
                        UserProfile userProfileObj = dbContext.UserProfiles.Find(userId);
                        userProfiles.Add(userProfileObj);
                    }
                }
            }
            catch (Exception)
            {
                userProfiles = null;
            }
            return userProfiles;
        }

        /// <summary>
        /// Adds Reply to the feedback as admin
        /// </summary>
        /// <param name="feedbackId">
        /// Stores the feedback id of the feedback to which reply is to be added
        /// </param>
        /// <param name="adminReply">
        /// Stores the reply to be added to the feedback as admin
        /// </param>
        /// <returns>
        /// True if reply is added successfully, false otherwise
        /// False if the feedbackId does not exist or any exception occurs
        /// </returns>
        public bool AddReplyToFeedback(int feedbackId, string adminReply)
        {
            bool status = false;

            Feedback feedback = null;

            try
            {
                feedback = dbContext.Feedbacks.FirstOrDefault(p => p.FeedbackId == feedbackId);

                if (feedback != null)
                {
                    feedback.AdminReply = adminReply;
                    feedback.AdminReplyTime = DateTime.Now;
                    dbContext.SaveChanges();
                    status = true;
                }
                else
                {
                    status = false;
                }

            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }

        /// <summary>
        /// Gets the Count of the users registered in the application with Role as User
        /// </summary>
        /// <returns>
        /// Count of the users registered in the application with Role as User
        /// </returns>
        public int GetUsersCount()
        {
            int count = -1;
            try
            {
                count = dbContext.Users.Where(user => user.RoleId == 1).Count();
            }
            catch (Exception)
            {

                count = -98;
            }
            return count;
        }

        /// <summary>
        /// Gets the Count of the online users registered in the application with Role as User
        /// </summary>
        /// <returns>
        /// Count of the online users registered in the application with Role as User
        /// </returns>
        public int GetActiveUsersCount()
        {
            int count = -1;
            try
            {
                count = (from s in dbContext.UserProfiles select GigaChatDBContext.ufn_GetActiveUserCount()).FirstOrDefault();
            }
            catch (Exception)
            {
                count = -98;
            }
            return count;
        }

        /// <summary>
        /// Gets All Feedback from the Feedback Table
        /// </summary>
        /// <returns>
        /// List of all the feedbacks from the Feedback Table,
        /// null if any exception occurs
        /// </returns>
        public List<Feedback> GetAllFeedbacks()
        {
            List<Feedback> lstFeedback = new List<Feedback>();
            try
            {
                lstFeedback = dbContext.Feedbacks.ToList();

            }
            catch (Exception)
            {

                lstFeedback = null;
            }
            return lstFeedback;
        }



    }
}
