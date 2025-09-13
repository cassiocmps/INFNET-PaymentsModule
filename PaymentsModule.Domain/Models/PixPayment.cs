namespace PaymentsModule.Domain.Models;

public class PixPayment : Payment
{
    public string QrCode { get; set; }
    public DateTime ExpirationDate { get; set; }
}
