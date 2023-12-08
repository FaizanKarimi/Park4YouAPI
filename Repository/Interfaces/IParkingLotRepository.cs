using System.Collections.Generic;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IParkingLotRepository : IBaseRepository
    {
        ParkingLots Get(int id);

        List<ParkingLots> Get(string searchStrnig);

        List<ParkingLotInformation> GetALL(string userId);

        List<ParkingLotInformation> GetALL();

        bool Update(ParkingLots parkingLot);

        bool Delete(ParkingLots parkingLot);

        ParkingLots Insert(ParkingLots parkingLot);
    }
}