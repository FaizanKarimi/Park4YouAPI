using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vehicles
{
    public class GetVehicleRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}