using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class ParkingDetailsRequest
    {
        [Required]
        public string RegistrationNumber { get; set; }
    }
}