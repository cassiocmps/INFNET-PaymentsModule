namespace PaymentsModule.Domain.Models;

public class Refund
{
    public Guid Id { get; set; }
    public DateTime RefundDate { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Payment Payment { get; set; } = null!;
}