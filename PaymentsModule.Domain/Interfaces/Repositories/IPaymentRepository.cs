using PaymentsModule.Domain.Models;

namespace PaymentsModule.Domain.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> UpdateAsync(Guid id, Payment payment);
    Task<bool> DeleteAsync(Guid id);
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
}