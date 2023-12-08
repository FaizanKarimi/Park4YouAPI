using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.UserProfile
{
    public class UpdateUserProfileRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetNumber { get; set; }
        public string ZipCode { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
    }
}