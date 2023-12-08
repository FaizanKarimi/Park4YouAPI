using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Area
{
    public class DeleteAreaRequest
    {
        [Required]
        public int? Id { get; set; }
    }
}