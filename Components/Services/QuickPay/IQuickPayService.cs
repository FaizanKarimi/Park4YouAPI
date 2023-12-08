using Infrastructure.DataModels;

namespace Components.Services.QuickPay
{
    public interface IQuickPayService
    {
        PaymentOrders ParkingPayment(string amount, string month, string year, string cvv, string cardNumber);

        void ValidateCard(string mobileNumber, string month, string year, string cvv, string cardNumber);
    }
}