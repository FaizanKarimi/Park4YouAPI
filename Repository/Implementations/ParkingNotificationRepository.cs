using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class ParkingNotificationRepository : IParkingNotificationRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public void Add(ParkingNotifications parkingNotifications)
        {
            const string sqlQuery = @"INSERT INTO ParkingNotifications (UserId, ParkingId, NotificationId15Min, NotificationId30Min, CreatedOn) 
                                      VALUES (@UserId, @ParkingId, @NotificationId15Min, @NotificationId30Min, @CreatedOn)";
            int result = UnitOfWork.Connection.Execute(sqlQuery, parkingNotifications, UnitOfWork.Transaction);
        }

        public ParkingNotifications Get(int parkingId)
        {
            const string sqlQuery = @"SELECT * FROM ParkingNotifications WHERE ParkingId = @ParkingId";
            return UnitOfWork.Connection.QueryFirstOrDefault<ParkingNotifications>(sqlQuery, new { ParkingId = parkingId });
        }

        public bool Update(ParkingNotifications parkingNotifications)
        {
            const string sqlQuery = @"UPDATE ParkingNotifications SET NotificationId15Min = @NotificationId15Min, NotificationId30Min = @NotificationId30Min WHERE ParkingId = @ParkingId";
            int result = UnitOfWork.Connection.Execute(sqlQuery, parkingNotifications, UnitOfWork.Transaction);
            return result > 0;
        }
    }
}