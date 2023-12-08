using Infrastructure.DataModels;

namespace Repository.Interfaces
{
    public interface IPaymentOrderRepository : IBaseRepository
    {
        PaymentOrders Add(PaymentOrders paymentOrder);
    }
}