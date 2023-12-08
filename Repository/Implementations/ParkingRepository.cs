using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;
using Dapper;
using System.Linq;
using Infrastructure.CustomModels;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Enums;

namespace Repository.Implementations
{
    public class ParkingRepository : IParkingRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public Parkings Add(Parkings parking)
        {
            const string sqlQuery = @"INSERT INTO Parkings (UserId, VendorParkingId, VehicleId, Name, ParkingLotId, AreaCode, RegistrationNumber, StartTime, StopTime, Price, IsFixed, MobileNumber, 
                                      IsLatest, ParkingStatusId, ParkingStatus, ParkingNote, TotalPrice, CardId) 
                                      VALUES (@UserId, @VendorParkingId, @VehicleId, @Name, @ParkingLotId, @AreaCode, @RegistrationNumber, @StartTime, @StopTime, @Price, @IsFixed, @MobileNumber, 
                                      @IsLatest, @ParkingStatusId, @ParkingStatus, @ParkingNote, @TotalPrice, @CardId)
                                      SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, new
            {
                UserId = parking.UserId,
                VendorParkingId = parking.VendorParkingId,
                VehicleId = parking.VehicleId,
                Name = parking.Name,
                ParkingLotId = parking.ParkingLotId,
                AreaCode = parking.AreaCode,
                RegistrationNumber = parking.RegistrationNumber,
                StartTime = parking.StartTime,
                StopTime = parking.StopTime,
                Price = parking.Price,
                IsFixed = parking.IsFixed,
                MobileNumber = parking.MobileNumber,
                IsLatest = parking.IsLatest,
                ParkingStatusId = parking.ParkingStatusId,
                ParkingStatus = parking.ParkingStatus,
                ParkingNote = parking.ParkingNote,
                TotalPrice = parking.TotalPrice,
                CardId = parking.CardId
            }, UnitOfWork.Transaction).Single();
            parking.Id = result;
            return parking;
        }

        public Parkings Get(int parkingId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE Id = @Id";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { Id = parkingId }, UnitOfWork.Transaction);
        }

        public Parkings GetParking(string registrationNumber)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE RegistrationNumber = @RegistrationNumber AND ParkingStatusId = @ParkingStatusId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { RegistrationNumber = registrationNumber, ParkingStatusId = (int)ParkingStatus.START });
        }

        public Parkings Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE MobileNumber = @MobileNumber AND ParkingStatusId = @ParkingStatusId ORDER BY Id DESC";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new
            {
                MobileNumber = mobileNumber,
                ParkingStatusId = (int)ParkingStatus.START
            });
        }

        public bool Delete(string mobileNumber)
        {
            const string sqlQuery = @"UPDATE Parkings SET IsDeleted = @IsDeleted WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new { IsDeleted = true, MobileNumber = mobileNumber }, UnitOfWork.Transaction);
            return result > 0;
        }

        public List<ParkingAroundAreas> GetParkingAroundAreas(string latitude, string longitude)
        {
            const string storeProcedureName = "GetParkingAroundAreas";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Latitude", latitude);
            parameters.Add("@Longitude", longitude);
            return UnitOfWork.Connection.Query<ParkingAroundAreas>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure).ToList();
        }

        public List<ParkingHistory> GetParkingHistory(string mobileNumber)
        {
            const string storedProcedureName = "GetParkingHistory";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@MobileNumber", mobileNumber);
            return UnitOfWork.Connection.Query<ParkingHistory>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure).ToList();
        }

        public ParkingReport GetParkingReport(int parkingId)
        {
            const string storedProcedureName = "GetParkingReciept";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ParkingId", parkingId);
            return UnitOfWork.Connection.QueryFirstOrDefault<ParkingReport>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public Parkings GetStartParkingByMobileNumber(string mobileNumber, int parkingStatusId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE MobileNumber = @MobileNumber AND ParkingStatusId = @ParkingStatusId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { MobileNumber = mobileNumber, ParkingStatusId = parkingStatusId });
        }

        public Parkings GetStartParkingByMobileNumber(string mobileNumber, int parkingStatusId, int cardId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE MobileNumber = @MobileNumber AND ParkingStatusId = @ParkingStatusId AND CardId = @CardId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { MobileNumber = mobileNumber, ParkingStatusId = parkingStatusId, CardId = cardId });
        }

        public Parkings GetStartParkingByVehicleId(string mobileNumber, int parkingStatusId, int vehicleId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE MobileNumber = @MobileNumber AND ParkingStatusId = @ParkingStatusId AND VehicleId = @VehicleId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { MobileNumber = mobileNumber, ParkingStatusId = parkingStatusId, VehicleId = vehicleId });
        }

        public Parkings GetStartParking(int parkingId, int parkingStatusId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE Id = @Id AND ParkingStatusId = @ParkingStatusId ORDER BY Id ASC";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { Id = parkingId, ParkingStatusId = parkingStatusId });
        }

        public Parkings GetStartedParking(string userId, int parkingStatusId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE UserId = @UserId AND ParkingStatusId = @ParkingStatusId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Parkings>(sqlQuery, new { UserId = userId, ParkingStatusId = parkingStatusId });
        }

        public bool Update(Parkings parking)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE Parkings SET 
                                             ParkingNote = @ParkingNote,
                                             ParkingStatusId = @ParkingStatusId,
                                             ParkingStatus = @ParkingStatus,
                                             StopTime = @StopTime,
                                             Price = @Price,
                                             CardId = @CardId
                                      WHERE  Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                ParkingNote = parking.ParkingNote,
                ParkingStatusId = parking.ParkingStatusId,
                ParkingStatus = parking.ParkingStatus,
                StopTime = parking.StopTime,
                Price = parking.Price,
                CardId = parking.CardId,
                Id = parking.Id
            }, UnitOfWork.Transaction);
            IsUpdated = result > 0;
            return IsUpdated;
        }

        public List<Parkings> GetStartedParkings(int parkingStatusId)
        {
            const string sqlQuery = @"SELECT * FROM Parkings WHERE ParkingStatusId = @ParkingStatusId";
            return UnitOfWork.Connection.Query<Parkings>(sqlQuery, new { ParkingStatusId = parkingStatusId }).ToList();
        }
    }
}