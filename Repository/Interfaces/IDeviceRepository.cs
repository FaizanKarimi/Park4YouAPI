using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IDeviceRepository : IBaseRepository
    {
        bool Add(Devices devices);

        bool Update(Devices devices);

        Devices Get(string mobileNumber);

        bool Delete(string mobileNumber);

        Devices GetByUserId(string userId);
    }
}