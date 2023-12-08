using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.User
{
    public class DeleteAccountRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}