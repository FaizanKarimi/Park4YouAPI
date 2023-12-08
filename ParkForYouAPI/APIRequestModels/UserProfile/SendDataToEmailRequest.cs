using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.UserProfile
{
    public class SendDataToEmailRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}