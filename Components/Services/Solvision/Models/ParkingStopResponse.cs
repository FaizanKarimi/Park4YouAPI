using System;

namespace Components.Services.Solvision.Models
{
    public class ParkingStopResponse
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public DateTime StopTime { get; set; }
    }
}