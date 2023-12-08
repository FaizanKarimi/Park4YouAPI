namespace Infrastructure.APIResponses.Parking
{
    public class ParkingHistoryResponse
    {
        public int ParkingId { get; set; }
        public string TotalPrice { get; set; }
        public string RegistrationNumber { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string VehicleName { get; set; }
        public string AreaCode { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public int? RemainingMinutes { get; set; }
    }
}