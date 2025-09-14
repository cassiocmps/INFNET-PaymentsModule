using System.ComponentModel.DataAnnotations;

namespace PaymentsModule.Client.Models
{
    public enum PaymentType
    {
        CreditCard = 1,
        Pix = 2,
        Boleto = 3
    }

    public class CreatePaymentRequest
    {
        [Required(ErrorMessage = "Order ID is required")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "Payment type is required")]
        public PaymentType? PaymentType { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public Guid? CardId { get; set; } // For credit card payments
    }

    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? QrCode { get; set; } // For PIX payments
        public string? Barcode { get; set; } // For Boleto payments
        public string? DigitableLine { get; set; } // For Boleto payments
        public DateTime? DueDate { get; set; } // For Boleto payments
        public DateTime? ExpirationDate { get; set; } // For PIX payments
    }

    public class RefundRequest
    {
        [Required(ErrorMessage = "Payment ID is required")]
        public Guid PaymentId { get; set; }

        [Required(ErrorMessage = "Refund reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        public BankAccountInfo? BankAccount { get; set; }
    }

    public class BankAccountInfo
    {
        public string Bank { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
    }

    public class RefundResponse
    {
        public Guid Id { get; set; }
        public DateTime RefundDate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}