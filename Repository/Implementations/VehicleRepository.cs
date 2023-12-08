using System.Collections.Generic;
using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public Vehicles Add(Vehicles vehicles)
        {
            const string sqlQuery = @"INSERT INTO Vehicles(UserId, RegistrationId, RegistrationNumber, MobileNumber, Name, IsLatest, IsDeleted, CreatedOn, UpdatedOn) 
                                            VALUES (@UserId, @RegistrationId, @RegistrationNumber, @MobileNumber, @Name, @IsLatest, @IsDeleted, @CreatedOn, @UpdatedOn)
                                            SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, new
            {
                UserId = vehicles.UserId,
                RegistrationId = vehicles.RegistrationId,
                RegistrationNumber = vehicles.RegistrationNumber,
                MobileNumber = vehicles.MobileNumber,
                Name = vehicles.Name,
                IsLatest = vehicles.IsLatest,
                IsDeleted = vehicles.IsDeleted,
                CreatedOn = vehicles.CreatedOn,
                UpdatedOn = vehicles.UpdatedOn
            }, UnitOfWork.Transaction).Single();
            vehicles.Id = result;
            return vehicles;
        }

        public bool Delete(Vehicles vehicles)
        {
            bool IsDeleted = false;
            const string sqlQuery = @"UPDATE Vehicles SET IsDeleted = @IsDeleted WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new { IsDeleted = vehicles.IsDeleted, Id = vehicles.Id }, UnitOfWork.Transaction);
            IsDeleted = result > 0;
            return IsDeleted;
        }

        public bool Delete(string mobileNumber)
        {
            const string sqlQuery = @"UPDATE Vehicles SET IsDeleted = @IsDeleted WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                IsDeleted = true,
                MobileNumber = mobileNumber
            }, UnitOfWork.Transaction);
            return result > 0;
        }

        public Vehicles Get(int vehicleId)
        {
            const string sqlQuery = @"SELECT * FROM Vehicles WHERE Id = @Id AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<Vehicles>(sqlQuery, new { Id = vehicleId, IsDeleted = false });
        }

        public Vehicles Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM Vehicles WHERE MobileNumber = @MobileNumber AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.QueryFirstOrDefault<Vehicles>(sqlQuery, new
            {
                MobileNumber = mobileNumber,
                IsDeleted = false
            });
        }

        public List<Vehicles> GetALL(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM Vehicles WHERE MobileNumber = @MobileNumber AND IsDeleted = @IsDeleted";
            return UnitOfWork.Connection.Query<Vehicles>(sqlQuery, new { MobileNumber = mobileNumber, IsDeleted = false }, UnitOfWork.Transaction).ToList();
        }                

        public bool Update(Vehicles vehicles)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE Vehicles SET 
                                        RegistrationId = @RegistrationId, 
                                        RegistrationNumber = @RegistrationNumber, 
                                        MobileNumber = @MobileNumber, 
                                        Name = @Name, 
                                        UpdatedOn = @UpdatedOn,
                                        IsLatest = @IsLatest,
                                        IsDeleted = @IsDeleted WHERE Id = @Id";
            UnitOfWork.Connection.Execute(sqlQuery, new
            {
                RegistrationId = vehicles.RegistrationId,
                RegistrationNumber = vehicles.RegistrationNumber,
                MobileNumber = vehicles.MobileNumber,
                Name = vehicles.Name,
                UpdatedOn = vehicles.UpdatedOn,
                IsLatest = vehicles.IsLatest,
                IsDeleted = vehicles.IsDeleted,
                Id = vehicles.Id
            }, UnitOfWork.Transaction);
            return IsUpdated;
        }

        public bool Update(List<Vehicles> vehicles)
        {
            const string sqlQuery = @"UPDATE Vehicles SET IsLatest = @IsLatest WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, vehicles, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}