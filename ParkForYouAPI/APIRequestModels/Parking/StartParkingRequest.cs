using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class StartParkingRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string RegistrationNumber { get; set; }
        [Required]
        public int Minutes { get; set; }
        [Required]
        public int CardId { get; set; }
        [Required]
        public int ParkingLotId { get; set; }

        public int ParkingId { get; set; }
        public string ParkingName { get; set; }
    }
}