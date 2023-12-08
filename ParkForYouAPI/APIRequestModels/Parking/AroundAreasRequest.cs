using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class AroundAreasRequest
    {
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }
    }
}