namespace PaymentsModule.Domain.Models;

public class BankAccount
{
    public string Bank { get; set; }
    public string Agency { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public string HolderName { get; set; }
}
