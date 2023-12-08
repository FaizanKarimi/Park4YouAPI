using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string RegistrationToken { get; set; }
    }
}