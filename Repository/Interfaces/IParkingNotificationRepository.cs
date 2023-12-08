using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IParkingNotificationRepository : IBaseRepository
    {
        ParkingNotifications Get(int parkingId);

        bool Update(ParkingNotifications parkingNotifications);

        void Add(ParkingNotifications parkingNotifications);
    }
}