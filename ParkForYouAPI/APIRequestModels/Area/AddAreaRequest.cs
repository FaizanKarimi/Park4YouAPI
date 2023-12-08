using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Area
{
    public class AddAreaRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string AreaCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Town { get; set; }
        [Required]
        public bool IsLatest { get; set; }
    }
}
