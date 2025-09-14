using Microsoft.EntityFrameworkCore;
using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Interfaces.Repositories;
using PaymentsModule.Domain.Models;
using PaymentsModule.Persistance.Data;
using PaymentsModule.Persistance.Entities;

namespace PaymentsModule.Persistance.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly PaymentsDbContext _context;

    public RefundRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<Refund?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Refunds
            .Include(r => r.Payment)
            .ThenInclude(p => p.Order)
            .Include(r => r.Payment)
            .ThenInclude(p => p.Card)
            .FirstOrDefaultAsync(r => r.Id == id);

        return entity is not null ? MapToModel(entity) : null;
    }

    public async Task<Refund?> GetByPaymentIdAsync(Guid paymentId)
    {
        var entity = await _context.Refunds
            .Include(r => r.Payment)
            .ThenInclude(p => p.Order)
            .Include(r => r.Payment)
            .ThenInclude(p => p.Card)
            .FirstOrDefaultAsync(r => r.PaymentId == paymentId);

        return entity is not null ? MapToModel(entity) : null;
    }

    public async Task<Refund> CreateAsync(Refund refund)
    {
        var entity = MapToEntity(refund);
        
        // Only generate new ID if not provided
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }
        
        entity.RefundDate = DateTime.UtcNow;

        _context.Refunds.Add(entity);
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    private static Refund MapToModel(RefundEntity entity)
    {
        return new Refund
        {
            Id = entity.Id,
            RefundDate = entity.RefundDate,
            Amount = entity.Amount,
            Reason = entity.Reason,
            Payment = MapPaymentToModel(entity.Payment)
        };
    }

    private static RefundEntity MapToEntity(Refund model)
    {
        return new RefundEntity
        {
            Id = model.Id,
            RefundDate = model.RefundDate,
            Amount = model.Amount,
            Reason = model.Reason,
            PaymentId = model.Payment.Id
        };
    }

    private static Payment MapPaymentToModel(PaymentEntity entity)
    {
        return entity.PaymentType switch
        {
            PaymentType.CREDIT_CARD => new CreditCardPayment
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = entity.LastUpdatedAt,
                Amount = entity.Amount,
                Status = entity.Status,
                Order = new Order { Id = entity.OrderId, Status = entity.Order.Status },
                Card = entity.Card is not null ? new Card
                {
                    Id = entity.Card.Id,
                    Number = entity.Card.Number,
                    Cvv = entity.Card.Cvv,
                    ExpirationDate = entity.Card.ExpirationDate,
                    HolderName = entity.Card.HolderName,
                    HolderDocument = entity.Card.HolderDocument
                } : null
            },
            PaymentType.PIX => new PixPayment
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = entity.LastUpdatedAt,
                Amount = entity.Amount,
                Status = entity.Status,
                Order = new Order { Id = entity.OrderId, Status = entity.Order.Status },
                QrCode = entity.QrCode ?? string.Empty,
                ExpirationDate = entity.PixExpirationDate ?? DateTime.UtcNow.AddDays(1)
            },
            PaymentType.BOLETO => new BoletoPayment
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = entity.LastUpdatedAt,
                Amount = entity.Amount,
                Status = entity.Status,
                Order = new Order { Id = entity.OrderId, Status = entity.Order.Status },
                Barcode = entity.Barcode ?? string.Empty,
                DueDate = entity.DueDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                DigitableLine = entity.DigitableLine ?? string.Empty
            },
            _ => throw new InvalidOperationException($"Unknown payment type: {entity.PaymentType}")
        };
    }
}