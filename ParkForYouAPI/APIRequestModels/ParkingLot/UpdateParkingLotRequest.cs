using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ParkingLot
{
    public class UpdateParkingLotRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string AreaCode { get; set; }
        [Required]
        public string CenterCoordinates { get; set; }
        [Required]
        public string GeoCoordinates { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
    }
}