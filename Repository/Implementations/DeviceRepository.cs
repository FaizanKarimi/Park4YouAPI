using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class DeviceRepository : IDeviceRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public bool Add(Devices devices)
        {
            bool IsAdded = false;
            string sqlQuery = @"INSERT INTO Devices (UserId, MobileNumber, DeviceToken, RegistrationToken, DeviceTypeId, CreatedOn, UpdatedOn)
                                VALUES (@UserId, @MobileNumber, @DeviceToken, @RegistrationToken, @DeviceTypeId, @CreatedOn, @UpdatedOn)";
            int result = UnitOfWork.Connection.Execute(sqlQuery, devices, UnitOfWork.Transaction);
            IsAdded = result > 0;
            return IsAdded;
        }

        public bool Update(Devices devices)
        {
            bool IsUpdated = false;
            const string sqlQuery = @"UPDATE Devices SET
                                        UserId = @UserId,
                                        MobileNumber = @MobileNumber,
                                        DeviceToken = @DeviceToken,
                                        RegistrationToken = @RegistrationToken,
                                        DeviceTypeId = @DeviceTypeId,
                                        UpdatedOn = @UpdatedOn WHERE Id = @Id";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                UserId = devices.UserId,
                MobileNumber = devices.MobileNumber,
                DeviceToken = devices.DeviceToken,
                RegistrationToken = devices.RegistrationToken,
                DeviceTypeId = devices.DeviceTypeId,
                UpdatedOn = devices.UpdatedOn,
                Id = devices.Id
            }, UnitOfWork.Transaction);
            return IsUpdated;
        }

        public Devices Get(string mobileNumber)
        {
            const string sqlQuery = @"SELECT * FROM Devices WHERE MobileNumber = @MobileNumber";
            return UnitOfWork.Connection.QueryFirstOrDefault<Devices>(sqlQuery, new
            {
                MobileNumber = mobileNumber
            }, UnitOfWork.Transaction);
        }

        public Devices GetByUserId(string userId)
        {
            const string sqlQuery = @"SELECT * FROM Devices WHERE UserId = @UserId";
            return UnitOfWork.Connection.QueryFirstOrDefault<Devices>(sqlQuery, new
            {
                UserId = userId
            }, UnitOfWork.Transaction);
        }

        public bool Delete(string mobileNumber)
        {
            const string sqlQuery = @"DELETE FROM Devices WHERE MobileNumber = @MobileNumber";
            int result = UnitOfWork.Connection.Execute(sqlQuery, new
            {
                MobileNumber = mobileNumber
            }, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}