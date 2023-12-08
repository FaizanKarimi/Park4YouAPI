namespace Components.Services.Interfaces
{
    public interface ITwillioService
    {
        bool SendMessage(string mobileNumber, string verificationCode);
    }
}