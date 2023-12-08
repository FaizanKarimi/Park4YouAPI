using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels
{
    public class AddDeviceRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string RegistrationToken { get; set; }
        [Required]
        public int DeviceTypeId { get; set; }
    }
}