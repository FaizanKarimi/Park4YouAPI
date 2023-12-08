using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class ChargeSheetRepository : IChargeSheetRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public ChargeSheets Add(ChargeSheets chargeSheet)
        {
            const string sqlQuery = @"INSERT INTO ChargeSheets (ParkingLotId, BaseCurrency, Name, UserId, IsDeleted, UpdatedBy, CreatedBy, CreatedOn, UpdatedOn)
                                      VALUES (@ParkingLotId, @BaseCurrency, @Name, @UserId, @IsDeleted, @UpdatedBy, @CreatedBy, @CreatedOn, @UpdatedOn)
                                      SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, chargeSheet, UnitOfWork.Transaction).Single();
            chargeSheet.Id = result;
            return chargeSheet;
        }

        public ChargeSheets Get(int id)
        {
            const string sqlQuery = @"SELECT * FROM ChargeSheets WHERE Id = @Id";
            return UnitOfWork.Connection.QueryFirstOrDefault<ChargeSheets>(sqlQuery, new
            {
                Id = id
            });
        }

        public ChargeSheets GetByParkingLotId(int parkingLotId)
        {
            const string sqlQuery = @"SELECT * FROM ChargeSheets WHERE ParkingLotId = @ParkingLotId AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<ChargeSheets>(sqlQuery, new { ParkingLotId = parkingLotId, IsDeleted = false });
        }

        public bool Update(ChargeSheets chargeSheet)
        {
            const string sqlQuery = @"UPDATE ChargeSheets SET Name = @Name, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                Name = chargeSheet.Name,
                UpdatedOn = chargeSheet.UpdatedOn,
                UpdatedBy = chargeSheet.UpdatedBy,
                Id = chargeSheet.Id
            }, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}