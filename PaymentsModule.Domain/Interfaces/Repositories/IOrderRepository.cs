using PaymentsModule.Domain.Models;

namespace PaymentsModule.Domain.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> CreateAsync(Order order);
    Task<Order?> UpdateAsync(Guid id, Order order);
    Task<bool> DeleteAsync(Guid id);
}