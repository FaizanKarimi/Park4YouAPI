using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Area
{
    public class GetAreaRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}