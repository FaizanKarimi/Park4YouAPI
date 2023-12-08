using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ParkingLot
{
    public class DeleteParkingLotRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
    }
}