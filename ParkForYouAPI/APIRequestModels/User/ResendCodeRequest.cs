using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class ResendCodeRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}