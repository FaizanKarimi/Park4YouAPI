namespace Infrastructure.CustomModels
{
    public class ParkingAroundAreas
    {
        public int ParkingLotId { get; set; }
        public string BasePrice { get; set; }
        public string AreaCode { get; set; }
        public string Name { get; set; }
        public string CenterCoordinates { get; set; }
        public string AreaGeoCoordinates { get; set; }
        public string CurrentCoordinate { get; set; }
        public float Distance { get; set; }
    }
}