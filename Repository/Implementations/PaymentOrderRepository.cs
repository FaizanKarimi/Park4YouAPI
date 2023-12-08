using System.Linq;
using Dapper;
using Infrastructure.DataModels;
using Repository.Interfaces;
using Repository.Provider;

namespace Repository.Implementations
{
    public class PaymentOrderRepository : IPaymentOrderRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public PaymentOrders Add(PaymentOrders paymentOrder)
        {
            const string sqlQuery = @"INSERT INTO PaymentOrders (ParkingId, PaymentOrderId, MerchantId, OrderId, Accepted, OrderType, TextOnStatement, BrandingId, Currency, State, 
                                      MetaDataType, Origin, Brand, Bin, Last4, ExpMonth, ExpYear, Country, Is3dSecure, IssuedTo, Hash, Number, CustomerIP, CustomerCountry, 
                                      FraudSuspected, NinCountryCode, NinGender, URL, AgreementId, Language, Amount, ContinueURL, CancelURL, CallbackURL, PaymentMethods, AutoFee, 
                                      AutoCapture, LinkBrandingId, Version, OperationId, OperationType, OperationAmount, Pending, QPStatusCode, QPStatusMessage, OperationCreatedDate,
                                      TestMode, Acquirer, CreatedAt, UpdatedAt, ReturnedAt, Balance, Fee, SystemCreatedDate) 
                                      VALUES (@ParkingId, @PaymentOrderId, @MerchantId, @OrderId, @Accepted, @OrderType, @TextOnStatement, @BrandingId, @Currency, @State, 
                                      @MetaDataType, @Origin, @Brand, @Bin, @Last4, @ExpMonth, @ExpYear, @Country, @Is3dSecure, @IssuedTo, @Hash, @Number, @CustomerIP, @CustomerCountry, 
                                      @FraudSuspected, @NinCountryCode, @NinGender, @URL, @AgreementId, @Language, @Amount, @ContinueURL, @CancelURL, @CallbackURL, @PaymentMethods, @AutoFee, 
                                      @AutoCapture, @LinkBrandingId, @Version, @OperationId, @OperationType, @OperationAmount, @Pending, @QPStatusCode, @QPStatusMessage, @OperationCreatedDate,
                                      @TestMode, @Acquirer, @CreatedAt, @UpdatedAt, @ReturnedAt, @Balance, @Fee, @SystemCreatedDate)
                                      SELECT CAST(SCOPE_IDENTITY() AS INT)";
            int result = UnitOfWork.Connection.Query<int>(sqlQuery, paymentOrder, UnitOfWork.Transaction).Single();
            paymentOrder.Id = result;
            return paymentOrder;            
        }
    }
}