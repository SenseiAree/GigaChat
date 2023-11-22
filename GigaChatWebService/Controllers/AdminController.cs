using GigaChatDALCrossPlatform;
using GigaChatDALCrossPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace GigaChatWebService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class AdminController : Controller
    {
        AdminRepository adminRepository;
        /// <summary>
        /// Constructor for AdminController used to initialise AdminRepository
        /// </summary>
        /// <param name="adminRepository"></param>
        public AdminController(AdminRepository adminRepository)
        {
            this.adminRepository = adminRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedBackId"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        [HttpPut]
        public bool AddReplyToFeedback(int feedBackId, string reply)
        {
            bool result = false;
            try
            {
                result = adminRepository.AddReplyToFeedback(feedBackId, reply);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        [HttpGet]
        public int GetUsersCount()
        {
            int count = -2;
            try
            {
                count = adminRepository.GetUsersCount();
            }
            catch (Exception)
            {
                count = -97;
            }
            return count;
        }

        [HttpGet]
        public JsonResult GetAllFeedbacks()
        {
            List<Models.Feedback> feedbacks = null;
            try
            {
                feedbacks = adminRepository.GetAllFeedbacks().Select(
                    feedback => new Models.Feedback()
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

        [HttpGet]
        public int GetActiveUsersCount()
        {
            int activeUserCount = -2;
            try
            {
                activeUserCount = adminRepository.GetActiveUsersCount();
            }
            catch (Exception)
            {
                activeUserCount = -97;
            }
            return activeUserCount;
        }

        [HttpGet]
        public double GetAverageRating()
        {
            double averageRating = -2;
            try
            {
                averageRating = adminRepository.GetAverageRating();
            }
            catch (Exception)
            {
                averageRating = -97;
            }
            return averageRating;
        }

        [HttpGet]
        public JsonResult GetAllFeedbackUsers()
        {
            List<Models.User> status = null;
            try
            {
                List<User> feedbackUsers = adminRepository.GetAllFeedbackUsers();
                if (feedbackUsers != null && feedbackUsers.Count > 0)
                {
                    status = feedbackUsers
                        .Select(feedbackUser => new Models.User()
                        {
                            DisplayName = feedbackUser.DisplayName,
                            EmailId = feedbackUser.EmailId,
                            UserId = feedbackUser.UserId,
                            DateOfBirth = feedbackUser.DateOfBirth,
                            Password = feedbackUser.Password,
                            RoleId = feedbackUser.RoleId
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                status = null;
            }
            return Json(status);
        }
        /// <summary>
        /// Get List User Profiles who have given feedbacks 
        /// </summary>
        /// <returns>
        /// List User Profiles who have given feedbacks, if 
        /// </returns>

        [HttpGet]
        public JsonResult GetAllFeedbackUserProfiles()
        {
            List<Models.UserProfile> status = null;
            try
            {
                List<UserProfile> userProfiles = adminRepository.GetAllFeedbackUserProfiles();
                if(userProfiles != null && userProfiles.Count > 0)
                {
                    status = userProfiles.Select( userProfile => new Models.UserProfile()
                    {
                        UserId = userProfile.UserId,
                        Avatar = userProfile.Avatar,
                        AvailabilityStatus = userProfile.AvailabilityStatus,
                        IsActive = userProfile.IsActive,
                        Theme = userProfile.Theme
                    }).ToList();
                }
            }
            catch (Exception)
            {
                status = null;
            }
            return Json(status);
        }
    }
}
