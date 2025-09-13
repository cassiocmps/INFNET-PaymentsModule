using PaymentsModule.Domain.Models;

namespace PaymentsModule.Domain.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id);
    Task<IEnumerable<Card>> GetAllAsync();
    Task<Card> CreateAsync(Card card);
    Task<Card?> UpdateAsync(Guid id, Card card);
    Task<bool> DeleteAsync(Guid id);
}