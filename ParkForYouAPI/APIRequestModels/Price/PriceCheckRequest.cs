using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Price
{
    public class PriceCheckRequest
    {
        [Required]
        public int? ParkingLotId { get; set; }
        [Required]
        public int? Minutes { get; set; }
    }
}