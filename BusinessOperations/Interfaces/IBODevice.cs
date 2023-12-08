using Infrastructure.DataModels;

namespace BusinessOperations.Interfaces
{
    public interface IBODevice
    {
        void Add(Devices devices);

        bool Update(string deviceToken, string registrationToken, string mobileNumber);
    }
}