using System.Collections.Generic;
using System.Linq;
using Dapper;
using Infrastructure.CustomModels;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class ParkingLotRepository : IParkingLotRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public bool Delete(ParkingLots parkingLot)
        {
            const string sqlQuery = "UPDATE ParkingLots SET IsDeleted = @IsDeleted, UpdatedBy = @UpdatedBy WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsDeleted = true,
                UpdatedBy = parkingLot.UpdatedBy,
                Id = parkingLot.Id
            }, UnitOfWork.Transaction);
            return result > 0;
        }

        public ParkingLots Get(int id)
        {
            const string sqlQuery = @"SELECT * FROM ParkingLots WHERE Id = @Id";
            return UnitOfWork.Connection.QueryFirstOrDefault<ParkingLots>(sqlQuery, new { Id = id });
        }

        public List<ParkingLots> Get(string searchString)
        {
            const string sqlQuery = @"SELECT * FROM ParkingLots WHERE IsDeleted = @IsDeleted AND (AreaCode LIKE @AreaCode OR Name LIKE @Name)";
            return UnitOfWork.Connection.Query<ParkingLots>(sqlQuery, new { IsDeleted = false, AreaCode = "%" + searchString + "%", Name = "%" + searchString + "%" }).ToList();
        }

        public List<ParkingLotInformation> GetALL(string userId)
        {
            const string sqlQuery = @"SELECT		PL.Id,
                                                    PL.Name,
			                                        PL.AreaCode,
			                                        PL.CenterCoordinates,
			                                        PL.GeoCoordinates,
			                                        PL.CreatedOn,
			                                        (UP.FirstName + ' ' + UP.LastName) AS UserName,
                                                    CS.Id AS ChargeSheetId
                                      FROM		    ParkingLots PL
                                                    INNER JOIN ChargeSheets CS ON PL.Id = CS.ParkingLotId
			                                        INNER JOIN UserProfiles UP ON PL.UserId = UP.UserId
                                      WHERE         PL.UserId = @UserId";
            return UnitOfWork.Connection.Query<ParkingLotInformation>(sqlQuery, new
            {
                UserId = userId
            }).ToList();
        }

        public List<ParkingLotInformation> GetALL()
        {
            const string sqlQuery = @"SELECT	PL.Id,
		                                        PL.Name,
		                                        PL.AreaCode,
		                                        PL.CenterCoordinates,
		                                        PL.GeoCoordinates,
		                                        PL.CreatedOn,
                                                PL.IsDeleted,
		                                        U.UserName,
                                                CS.Id AS ChargeSheetId
                                      FROM	    ParkingLots PL
                                                INNER JOIN ChargeSheets CS ON PL.Id = CS.ParkingLotId
                                                LEFT JOIN AspNetUsers U ON PL.UserId = U.Id";
            return UnitOfWork.Connection.Query<ParkingLotInformation>(sqlQuery).ToList();
        }

        public ParkingLots Insert(ParkingLots parkingLot)
        {
            const string sqlQuery = @"INSERT INTO ParkingLots (UserId, MobileNumber, Name, AreaCode, CenterCoordinates, GeoCoordinates, UpdatedBy, IsDeleted, CreatedOn, UpdatedOn)
                                    VALUES (@UserId, @MobileNumber, @Name, @AreaCode, @CenterCoordinates, @GeoCoordinates, @UpdatedBy, @IsDeleted, @CreatedOn, @UpdatedOn)
                                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, parkingLot, UnitOfWork.Transaction).Single();
            parkingLot.Id = result;
            return parkingLot;
        }

        public bool Update(ParkingLots parkingLot)
        {
            const string sqlQuery = @"UPDATE ParkingLots SET 
                                            Name = @Name, 
                                            AreaCode = @AreaCode, 
                                            CenterCoordinates = @CenterCoordinates, 
                                            GeoCoordinates = @GeoCoordinates, 
                                            IsDeleted = @IsDeleted,
                                            UpdatedOn = @UpdatedOn,
                                            UpdatedBy = @UpdatedBy
                                      WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, parkingLot, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}