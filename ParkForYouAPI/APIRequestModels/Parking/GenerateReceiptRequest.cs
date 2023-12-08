using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class GenerateReceiptRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public int? ParkingId { get; set; }
    }
}