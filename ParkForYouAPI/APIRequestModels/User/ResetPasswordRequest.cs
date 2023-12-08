using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class ResetPasswordRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}