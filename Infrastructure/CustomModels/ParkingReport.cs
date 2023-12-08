using System;

namespace Infrastructure.CustomModels
{
    public class ParkingReport
    {
        public int ParkingId { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public string ParkingNote { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public bool IsFixed { get; set; }
        public int ParkingStatusId { get; set; }
        public string ParkingStatus { get; set; }
        public decimal Price { get; set; }
        public string OrderId { get; set; }
        public string QPStatusMessage { get; set; }
        public string Brand { get; set; }
        public string Bin { get; set; }
        public string Last4 { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
    }
}