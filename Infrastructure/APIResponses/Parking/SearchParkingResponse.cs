using System.Collections.Generic;
using Infrastructure.CustomModels;

namespace Infrastructure.APIResponses.Parking
{
    public class SearchParkingResponse
    {
    }

    public class SearchResultBindModel
    {
        public int ParkingLotId { get; set; }
        public string AreaCode { get; set; }
        public string Name { get; set; }
        public string CenterPoint { get; set; }
        public string AreaGeoCordinates { get; set; }
    }

    public class ParkingLotModel
    {
        public int ParkingLotId { get; set; }
        public string AreaCode { get; set; }
        public string Name { get; set; }
        public string CenterPoint { get; set; }
        public string BasePrice { get; set; }
        public float Distance { get; set; }
        public List<LongitudeLatitudeModel> AreaGeoCoordinates { get; set; }
    }
}