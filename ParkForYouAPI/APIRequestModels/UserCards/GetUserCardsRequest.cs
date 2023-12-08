using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.UserCards
{
    public class GetUserCardsRequest
    {
        [Required]
        public string MobileNumber { get; set; }
    }
}