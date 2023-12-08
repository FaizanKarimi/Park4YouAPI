using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Vehicles
{
    public class UpdateVehicleRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string RegistrationNumber { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsLatest { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}