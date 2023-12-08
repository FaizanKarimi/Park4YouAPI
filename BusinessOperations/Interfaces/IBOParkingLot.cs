using System.Collections.Generic;
using Infrastructure.APIResponses.Parking;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBOParkingLot
    {
        ParkingLots Get(int id);

        string GetParkingPrice(int parkingLotId, int minutes);

        List<ParkingLotModel> Get(string searchString);                

        List<ParkingLotInformation> GetALL(string userId, string role);

        bool Update(ParkingLots parkingLot);

        bool Delete(int id);

        bool Save(ParkingLots parkingLot, ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrice);
    }
}