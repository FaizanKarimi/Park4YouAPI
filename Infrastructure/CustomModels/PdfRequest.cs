namespace Infrastructure.CustomModels
{
    public class PdfRequest
    {
        public string OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bin { get; set; }
        public string Last { get; set; }
        public string Country { get; set; }
        public string Brand { get; set; }
        public string Town { get; set; }
        public string QPStatusMessage { get; set; }
        public string Price { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string ParkingNote { get; set; }
        public string FileName { get; set; }
    }
}