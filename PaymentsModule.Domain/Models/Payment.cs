using PaymentsModule.Domain.Enums;

namespace PaymentsModule.Domain.Models;

public abstract class Payment
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public Order Order { get; set; }
}
