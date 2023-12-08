using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels
{
    public class AddVehicleRequest
    {
        public string RegistrationId { get; set; }
        [Required]
        public string RegistrationNumber { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        [Required]
        public bool IsLatest { get; set; }
    }
}