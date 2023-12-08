using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IChargeSheetRepository : IBaseRepository
    {
        ChargeSheets GetByParkingLotId(int parkingLotId);

        ChargeSheets Get(int id);

        bool Update(ChargeSheets chargeSheet);

        ChargeSheets Add(ChargeSheets chargeSheet);
    }
}