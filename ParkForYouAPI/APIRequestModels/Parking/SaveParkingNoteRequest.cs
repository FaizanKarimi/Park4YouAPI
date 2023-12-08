using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.Parking
{
    public class SaveParkingNoteRequest
    {
        [Required]
        public int? ParkingId { get; set; }
        [Required]
        public string ParkingNote { get; set; }
    }
}