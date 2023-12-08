using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels
{
    public class AddUserCardRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardVerficationValue { get; set; }
        [Required]
        public string PaymentType { get; set; }
        [Required]
        public bool IsDefault { get; set; }                
        public string CardExpiry { get; set; }
    }
}