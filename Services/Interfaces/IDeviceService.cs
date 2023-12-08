using Infrastructure.DataModels;

namespace Services.Interfaces
{
    public interface IDeviceService
    {
        bool Add(Devices devices);

        bool Update(Devices devices);

        Devices Get(string mobileNumber);

        Devices GetByUserId(string userId);
    }
}