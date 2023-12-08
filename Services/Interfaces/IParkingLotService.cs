using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IParkingLotService
    {
        ParkingLots Get(int id);

        List<ParkingLots> Get(string searchString);

        List<ParkingLotInformation> GetParkingLots(string userId);

        List<ParkingLotInformation> GetALL();

        bool Update(ParkingLots parkingLot);

        bool Delete(int id);

        bool Save(ParkingLots parkingLot, ChargeSheets chargeSheet, List<ChargeSheetPrices> chargeSheetPrices);
    }
}