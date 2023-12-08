using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class SearchParkingRequest
    {
        [Required]
        public string SearchString { get; set; }
    }
}