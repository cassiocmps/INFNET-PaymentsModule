using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Models;

namespace PaymentsModule.API.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public Order Order { get; set; } = new();
        public string PaymentType { get; set; } = string.Empty;
        
        // Credit Card specific fields (when applicable)
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}