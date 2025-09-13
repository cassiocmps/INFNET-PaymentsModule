namespace PaymentsModule.Domain.Models;

public class Card
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string Cvv { get; set; }
    public string ExpirationDate { get; set; }
    public string HolderName { get; set; }
    public string HolderDocument { get; set; }
}
