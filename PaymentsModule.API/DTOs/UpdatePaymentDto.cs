using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Models;

namespace PaymentsModule.API.DTOs
{
    public class UpdatePaymentDto
    {
        public decimal? Amount { get; set; }
        public PaymentStatus? Status { get; set; }
    }
}