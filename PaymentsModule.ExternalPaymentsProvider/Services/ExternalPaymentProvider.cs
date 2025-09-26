using System.Text.RegularExpressions;
using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Models;
using PaymentsModule.ExternalPaymentsProvider.Interfaces;

namespace PaymentsModule.ExternalPaymentsProvider.Services;

public class ExternalPaymentProvider : IExternalPaymentProvider
{
    public async Task<CreditCardPayment> ChargeCardPaymentAsync(decimal amount, Card card)
    {
        await Task.Delay(500); // Simulate network latency
        var cardPayment = new CreditCardPayment
        {
            Id = Guid.NewGuid(),
            Card = card,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
        
        var random = new Random();
        var chargeSuccess = random.Next(0, 2) == 1; // 50% chance of success
        cardPayment.Status = chargeSuccess ? PaymentStatus.APPROVED : PaymentStatus.REFUSED;
        return cardPayment;
    }

    public async Task<Guid> RefundCardPaymentAsync(decimal amount, Card card)
    {
        await Task.Delay(500); // Simulate network latency
        
        var random = new Random();
        var refundSuccess = random.Next(0, 10) != 0; // 90% chance of success (higher than charges)
        
        // Return a refund transaction ID if successful, empty GUID if failed
        return refundSuccess ? Guid.NewGuid() : Guid.Empty;
    }

    public async Task<PixPayment> CreatePixPaymentAsync(decimal amount)
    {
        await Task.Delay(500); // Simulate network latency
        var pixPayment = new PixPayment
        {
            Id = Guid.NewGuid(),
            QrCode = "simulated-qrcode",
            ExpirationDate = DateTime.UtcNow.AddMinutes(30),
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            Status = PaymentStatus.PENDING
        };
        return pixPayment;
    }

    public async Task<BoletoPayment> CreateBoletoPaymentAsync(decimal amount)
    {
        await Task.Delay(500); // Simulate network latency
        var boletoPayment = new BoletoPayment
        {
            Id = Guid.NewGuid(),
            Barcode = "simulated-barcode",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            DigitableLine = "simulated-digitable-line",
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            Status = PaymentStatus.PENDING
        };
        return boletoPayment;
    }

    public async Task<Guid> CreateDepositAsync(decimal amount, BankAccount bankAccount)
    {
        await Task.Delay(500); // Simulate network latency
        
        var random = new Random();
        var depositSuccess = random.Next(0, 4) != 0; // 75% chance of success (higher than payment charges)
        
        // Return deposit transaction ID if successful, empty GUID if failed
        return depositSuccess ? Guid.NewGuid() : Guid.Empty;
    }

    public async Task<bool> ValidateCardAsync(Card card)
    {
        await Task.Delay(200); // Simulate network latency for validation
        
        if (!IsValidCardNumber(card.Number))
            return false;
            
        if (!IsValidCvv(card.Cvv))
            return false;
            
        if (!IsValidExpirationDate(card.ExpirationDate))
            return false;
            
        if (string.IsNullOrWhiteSpace(card.HolderName) || card.HolderName.Length < 2 || card.HolderName.Length > 100)
            return false;
            
        if (!IsValidDocument(card.HolderDocument))
            return false;
            
        // Simulate some cards being rejected by the payment provider (10% rejection rate)
        var random = new Random();
        var providerRejects = random.Next(0, 10) == 0;
        
        return !providerRejects;
    }
    
    private static bool IsValidCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;
            
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");
        
        if (!Regex.IsMatch(cardNumber, @"^\d{16}$"))
            return false;
            
        return IsValidLuhn(cardNumber);
    }
    
    private static bool IsValidLuhn(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;
        
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());
            
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit = (digit % 10) + 1;
            }
            
            sum += digit;
            alternate = !alternate;
        }
        
        return sum % 10 == 0;
    }
    
    private static bool IsValidCvv(string cvv)
    {
        return !string.IsNullOrWhiteSpace(cvv) && Regex.IsMatch(cvv, @"^\d{3,4}$");
    }
    
    private static bool IsValidExpirationDate(string expirationDate)
    {
        if (string.IsNullOrWhiteSpace(expirationDate))
            return false;

        var regex = new Regex(@"^(0[1-9]|1[0-2])-\d{2}$");
        if (!regex.IsMatch(expirationDate))
            return false;

        try
        {
            var parts = expirationDate.Split('-');
            var month = int.Parse(parts[0]);
            var year = 2000 + int.Parse(parts[1]);

            var expirationDateObj = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            
            return expirationDateObj >= DateTime.Now.Date;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool IsValidDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return false;
            
        // Remove formatting characters
        document = document.Replace(".", "").Replace("-", "").Replace("/", "");
        
        // Check if it's a valid CPF (11 digits) or CNPJ (14 digits)
        return Regex.IsMatch(document, @"^\d{11}$") || Regex.IsMatch(document, @"^\d{14}$");
    }
}