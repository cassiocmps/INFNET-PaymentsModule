namespace PaymentsModule.API.DTOs
{
    public class CreatePaymentDto
    {
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        
        // Credit Card specific fields
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}