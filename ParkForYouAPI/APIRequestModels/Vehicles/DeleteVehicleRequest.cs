using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vehicles
{
    public class DeleteVehicleRequest
    {
        [Required]
        public int? VehicleId { get; set; }
    }
}