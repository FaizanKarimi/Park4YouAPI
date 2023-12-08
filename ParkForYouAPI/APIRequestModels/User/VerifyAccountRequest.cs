using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class VerifyAccountRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string VerificationCode { get; set; }
    }
}