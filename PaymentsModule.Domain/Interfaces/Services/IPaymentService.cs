using PaymentsModule.Domain.Models;
using PaymentsModule.Domain.Enums;

namespace PaymentsModule.Domain.Interfaces.Services;

public interface IPaymentService
{
    Task<CreditCardPayment> CreateCardPaymentChargeAsync(Guid orderId, Guid cardId, decimal amount);
    Task<PixPayment> CreatePixPaymentAsync(Guid orderId, decimal amount);
    Task<BoletoPayment> CreateBoletoPaymentAsync(Guid orderId, decimal amount);
    Task<Payment> ReissuePaymentAsync(Guid paymentId);
    Task<Guid> RefundPaymentAsync(Guid paymentId, BankAccount bankAccount);
}