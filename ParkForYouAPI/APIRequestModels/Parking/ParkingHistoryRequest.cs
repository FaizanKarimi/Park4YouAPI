using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class ParkingHistoryRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}