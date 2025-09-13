namespace PaymentsModule.Domain.Models;

public class CreditCardPayment : Payment
{
    public Card Card { get; set; }
}