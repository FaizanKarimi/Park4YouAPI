using System;

namespace Components.Services.Price
{
    public class CustomHours
    {
        public DateTime startHour { get; set; }
        public DateTime endHour { get; set; }
        public int minutes { get; set; }
        public decimal rate { get; set; }
    }
}