namespace Infrastructure.APIResponses.Parking
{
    public class ActiveParkingDetailResponse
    {
        public string MobileNumber { get; set; }
        public string ParkingLotName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Price { get; set; }
        public string ParkingNote { get; set; }
        public string AreaCode { get; set; }
    }
}