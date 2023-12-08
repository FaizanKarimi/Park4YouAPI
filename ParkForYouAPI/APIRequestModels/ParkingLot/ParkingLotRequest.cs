using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ParkingLot
{
    public class ParkingLotRequest
    {
        [Required]
        public string Name { get; set; }
    }
}