using Microsoft.EntityFrameworkCore;
using PaymentsModule.Domain.Models;
using PaymentsModule.Domain.Enums;
using PaymentsModule.Persistance.Data;
using PaymentsModule.Persistance.Entities;
using PaymentsModule.Domain.Interfaces.Repositories;

namespace PaymentsModule.Persistance.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDbContext _context;

    public PaymentRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Card)
            .FirstOrDefaultAsync(p => p.Id == id);

        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        var entities = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Card)
            .ToListAsync();

        return entities.Select(MapToModel);
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        var entity = MapToEntity(payment);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.LastUpdatedAt = DateTime.UtcNow;

        _context.Payments.Add(entity);
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    public async Task<Payment?> UpdateAsync(Guid id, Payment payment)
    {
        var entity = await _context.Payments.FindAsync(id);
        if (entity == null) return null;

        // Update properties
        entity.Amount = payment.Amount;
        entity.Status = payment.Status;
        entity.LastUpdatedAt = DateTime.UtcNow;
        
        // Update payment-specific fields based on type
        if (payment is CreditCardPayment cardPayment)
        {
            entity.CardId = cardPayment.Card?.Id;
        }
        else if (payment is PixPayment pixPayment)
        {
            entity.QrCode = pixPayment.QrCode;
            entity.PixExpirationDate = pixPayment.ExpirationDate;
        }
        else if (payment is BoletoPayment boletoPayment)
        {
            entity.Barcode = boletoPayment.Barcode;
            entity.DueDate = boletoPayment.DueDate;
            entity.DigitableLine = boletoPayment.DigitableLine;
        }

        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Payments.FindAsync(id);
        if (entity == null) return false;

        _context.Payments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        var entity = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.Card)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        return entity != null ? MapToModel(entity) : null;
    }

    private static Payment MapToModel(PaymentEntity entity)
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
                Card = entity.Card != null ? new Card
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

    private static PaymentEntity MapToEntity(Payment model)
    {
        var entity = new PaymentEntity
        {
            Id = model.Id,
            CreatedAt = model.CreatedAt,
            LastUpdatedAt = model.LastUpdatedAt,
            Amount = model.Amount,
            Status = model.Status,
            OrderId = model.Order.Id
        };

        if (model is CreditCardPayment cardPayment)
        {
            entity.PaymentType = PaymentType.CREDIT_CARD;
            entity.CardId = cardPayment.Card?.Id;
        }
        else if (model is PixPayment pixPayment)
        {
            entity.PaymentType = PaymentType.PIX;
            entity.QrCode = pixPayment.QrCode;
            entity.PixExpirationDate = pixPayment.ExpirationDate;
        }
        else if (model is BoletoPayment boletoPayment)
        {
            entity.PaymentType = PaymentType.BOLETO;
            entity.Barcode = boletoPayment.Barcode;
            entity.DueDate = boletoPayment.DueDate;
            entity.DigitableLine = boletoPayment.DigitableLine;
        }

        return entity;
    }
}