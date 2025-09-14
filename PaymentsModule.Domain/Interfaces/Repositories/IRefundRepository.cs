using PaymentsModule.Domain.Models;

namespace PaymentsModule.Domain.Interfaces.Repositories;

public interface IRefundRepository
{
    Task<Refund?> GetByIdAsync(Guid id);
    Task<Refund?> GetByPaymentIdAsync(Guid paymentId);
    Task<Refund> CreateAsync(Refund refund);
}