using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class RegisterRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string RegistrationToken { get; set; }
        [Required]
        public string Password { get; set; }
        public string Language { get; set; }
        [Required]
        public int? DeviceTypeId { get; set; }
    }
}