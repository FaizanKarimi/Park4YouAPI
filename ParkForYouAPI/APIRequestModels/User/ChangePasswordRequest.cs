using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class ChangePasswordRequest
    {
        [Required]
        public string MobileNumber { get; set; }        
        [Required]
        public string NewPassword { get; set; }
    }
}