using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vendors
{
    public class AddVendorRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}