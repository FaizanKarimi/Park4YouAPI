using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.UserCards
{
    public class DefaultUserCardRequest
    {
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public int? CardId { get; set; }
    }
}