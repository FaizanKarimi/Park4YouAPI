using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vendors
{
    public class DeleteVendorRequest
    {
        [Required]
        public string UserId { get; set; }
        public bool Status { get; set; }
    }
}