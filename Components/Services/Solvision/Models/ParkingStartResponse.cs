using System;

namespace Components.Services.Solvision.Models
{
    public class ParkingStartResponse
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public DateTime StartTime { get; set; }
    }
}