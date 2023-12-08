using System.Collections.Generic;

namespace Components.Services.Price
{
    public class DurationBaseRates
    {
        public DurationBaseRates()
        {
            hourPrices = new List<HourPrices>();
        }

        public string dayOfWeek { get; set; }
        public List<HourPrices> hourPrices { get; set; }
    }

    public class HourPrices
    {
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public decimal ratePerHour { get; set; }
    }
}