using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ChargeSheet
{
    public class SaveChargeSheetRequest
    {
        [Required]
        public string Name { get; set; }
    }
}