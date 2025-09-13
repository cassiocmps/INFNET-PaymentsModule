using Microsoft.EntityFrameworkCore;
using PaymentsModule.Domain.Interfaces.Repositories;
using PaymentsModule.Domain.Models;
using PaymentsModule.Persistance.Data;
using PaymentsModule.Persistance.Entities;

namespace PaymentsModule.Persistance.Repositories;

public class CardRepository : ICardRepository
{
    private readonly PaymentsDbContext _context;

    public CardRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<Card?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Cards.FindAsync(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<Card>> GetAllAsync()
    {
        var entities = await _context.Cards.ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<Card> CreateAsync(Card card)
    {
        var entity = MapToEntity(card);
        entity.Id = Guid.NewGuid();

        _context.Cards.Add(entity);
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    public async Task<Card?> UpdateAsync(Guid id, Card card)
    {
        var entity = await _context.Cards.FindAsync(id);
        if (entity == null) return null;

        entity.Number = card.Number;
        entity.Cvv = card.Cvv;
        entity.ExpirationDate = card.ExpirationDate;
        entity.HolderName = card.HolderName;
        entity.HolderDocument = card.HolderDocument;

        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Cards.FindAsync(id);
        if (entity == null) return false;

        _context.Cards.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static Card MapToModel(CardEntity entity)
    {
        return new Card
        {
            Id = entity.Id,
            Number = entity.Number,
            Cvv = entity.Cvv,
            ExpirationDate = entity.ExpirationDate,
            HolderName = entity.HolderName,
            HolderDocument = entity.HolderDocument
        };
    }

    private static CardEntity MapToEntity(Card model)
    {
        return new CardEntity
        {
            Id = model.Id,
            Number = model.Number,
            Cvv = model.Cvv,
            ExpirationDate = model.ExpirationDate,
            HolderName = model.HolderName,
            HolderDocument = model.HolderDocument
        };
    }
}