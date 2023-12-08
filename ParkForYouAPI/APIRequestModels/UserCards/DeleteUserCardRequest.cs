using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels
{
    public class DeleteUserCardRequest
    {
        [Required]
        public int? CardId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}