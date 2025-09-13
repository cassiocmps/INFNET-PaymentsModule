namespace PaymentsModule.Domain.Models;

public class BoletoPayment : Payment
{
    public string Barcode { get; set; }
    public DateOnly DueDate { get; set; }
    public string DigitableLine { get; set; }
}
