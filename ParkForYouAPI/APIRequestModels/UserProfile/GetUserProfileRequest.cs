using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.UserProfile
{
    public class GetUserProfileRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}