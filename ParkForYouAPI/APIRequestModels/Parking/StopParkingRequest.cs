using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class StopParkingRequest
    {
        [Required]
        public int? ParkingId { get; set; }
        public bool IsAutoStopFlow { get; set; }
        [Required]
        public int? RemainingSeconds { get; set; }
    }
}