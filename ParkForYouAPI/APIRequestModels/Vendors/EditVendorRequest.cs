using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vendors
{
    public class EditVendorRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }                
        [Required]
        public string MobileNumber { get; set; }
        public string Password { get; set; }
    }
}