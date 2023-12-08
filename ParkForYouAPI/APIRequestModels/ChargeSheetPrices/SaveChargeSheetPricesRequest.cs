using System.ComponentModel.DataAnnotations;

namespace ParkForYouAPI.APIRequestModels.ChargeSheetPrices
{
    public class SaveChargeSheetPricesRequest
    {
        [Required]
        public string AttributeKey { get; set; }
        [Required]
        public string AttributeValue { get; set; }
    }
}