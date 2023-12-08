namespace Infrastructure.APIResponses.Parking
{
    public class StopParkingResponse
    {
        public int ParkingId { get; set; }
        public int ParkingLotId { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string AreaCode { get; set; }
        public string ParkingName { get; set; }
        public string RegistrationNumber { get; set; }
        public string TotalPrice { get; set; }
        public string VehicleName { get; set; }
        public string Note { get; set; }
        public int BasePrice { get; set; }
        public bool IsFixed { get; set; }
        public string CenterPoint { get; set; }
        public int RemainingMinutes { get; set; }
        public int TotalMinutes { get; set; }
    }
}