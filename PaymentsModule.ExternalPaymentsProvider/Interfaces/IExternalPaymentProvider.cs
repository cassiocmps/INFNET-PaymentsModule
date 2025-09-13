using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Models;

namespace PaymentsModule.ExternalPaymentsProvider.Interfaces;

public interface IExternalPaymentProvider
{
    Task<CreditCardPayment> ChargeCardPaymentAsync(decimal amount, Card card);
    Task<Guid> RefundCardPaymentAsync(decimal amount, Card card);
    Task<PixPayment> CreatePixPaymentAsync(decimal amount);
    Task<BoletoPayment> CreateBoletoPaymentAsync(decimal amount);
    Task<Guid> CreateDepositAsync(decimal amount, BankAccount bankAccount);
    Task<bool> ValidateCardAsync(Card card);
}