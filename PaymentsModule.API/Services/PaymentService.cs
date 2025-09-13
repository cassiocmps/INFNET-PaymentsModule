using PaymentsModule.Domain.Models;
using PaymentsModule.Domain.Enums;
using PaymentsModule.Domain.Interfaces.Repositories;
using PaymentsModule.Domain.Interfaces.Services;
using PaymentsModule.ExternalPaymentsProvider.Interfaces;

namespace PaymentsModule.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IExternalPaymentProvider _externalPaymentProvider;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        ICardRepository cardRepository,
        IExternalPaymentProvider externalPaymentProvider)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _cardRepository = cardRepository;
        _externalPaymentProvider = externalPaymentProvider;
    }

    public async Task<CreditCardPayment> CreateCardPaymentChargeAsync(Guid orderId, Guid cardId, decimal amount)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card is null)
        {
            throw new ArgumentException($"Card with ID {cardId} not found");
        }

        var existingOrder = await _orderRepository.GetByIdAsync(orderId);
        if (existingOrder is not null)
        {
            throw new ArgumentException($"Order ID {orderId} already exists. Please use ReissuePayment.");
        }

        var cardPayment = await _externalPaymentProvider.ChargeCardPaymentAsync(amount, card);
        
        cardPayment.Order = new Order
        {
            Id = orderId,
            Status = cardPayment.Status == PaymentStatus.APPROVED
                ? OrderStatus.PAID
                : OrderStatus.FAILED
        };

        await _paymentRepository.CreateAsync(cardPayment);

        return cardPayment;
    }

    public async Task<PixPayment> CreatePixPaymentAsync(Guid orderId, decimal amount)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(orderId);
        if (existingOrder is not null)
        {
            throw new ArgumentException($"Order ID {orderId} already exists. Please use ReissuePayment.");
        }

        var pixPayment = await _externalPaymentProvider.CreatePixPaymentAsync(amount);
        
        pixPayment.Order = new Order
        {
            Id = orderId,
            Status = OrderStatus.AWAITING_PAYMENT
        };

        await _paymentRepository.CreateAsync(pixPayment);

        return pixPayment;
    }

    public async Task<BoletoPayment> CreateBoletoPaymentAsync(Guid orderId, decimal amount)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(orderId);
        if (existingOrder is not null)
        {
            throw new ArgumentException($"Order ID {orderId} already exists. Please use ReissuePayment.");
        }

        var boletoPayment = await _externalPaymentProvider.CreateBoletoPaymentAsync(amount);
        
        boletoPayment.Order = new Order
        {
            Id = orderId,
            Status = OrderStatus.AWAITING_PAYMENT
        };

        await _paymentRepository.CreateAsync(boletoPayment);

        return boletoPayment;
    }

    public async Task<Payment> ReissuePaymentAsync(Guid paymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment is null)
        {
            throw new ArgumentException($"Payment with ID {paymentId} not found");
        }

        if (payment.Status != PaymentStatus.FAILED && payment.Status != PaymentStatus.REFUSED)
        {
            throw new InvalidOperationException($"Payment with status {payment.Status} cannot be reissued");
        }

        Payment newPayment = payment switch
        {
            CreditCardPayment cardPayment => await _externalPaymentProvider.ChargeCardPaymentAsync(cardPayment.Amount, cardPayment.Card),
            PixPayment pixPayment => await _externalPaymentProvider.CreatePixPaymentAsync(pixPayment.Amount),
            BoletoPayment boletoPayment => await _externalPaymentProvider.CreateBoletoPaymentAsync(boletoPayment.Amount),
            _ => throw new InvalidOperationException($"Unsupported payment type: {payment.GetType().Name}")
        };

        newPayment.Order = payment.Order;

        payment.Status = PaymentStatus.REISSUED;
        payment.LastUpdatedAt = DateTime.UtcNow;
        await _paymentRepository.UpdateAsync(payment.Id, payment);

        await _paymentRepository.CreateAsync(newPayment);

        return newPayment;
    }

    public async Task<Guid> RefundPaymentAsync(Guid paymentId, BankAccount bankAccount)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment is null)
        {
            throw new ArgumentException($"Payment with ID {paymentId} not found");
        }

        if (payment.Status != PaymentStatus.APPROVED)
        {
            throw new InvalidOperationException($"Payment with status {payment.Status} cannot be refunded");
        }

        if (bankAccount is null)
        {
            throw new ArgumentException("Bank account information is required for refund");
        }

        Guid refundTransactionId;

        if (payment is CreditCardPayment cardPayment)
        {
            refundTransactionId = await _externalPaymentProvider.RefundCardPaymentAsync(payment.Amount, cardPayment.Card);
        }
        else
        {
            refundTransactionId = await _externalPaymentProvider.CreateDepositAsync(payment.Amount, bankAccount);
        }

        // Update payment status based on refund result
        if (refundTransactionId != Guid.Empty)
        {
            payment.Status = PaymentStatus.REFUNDED;
            payment.LastUpdatedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment.Id, payment);
            return refundTransactionId;
        }
        else
        {
            throw new InvalidOperationException("Refund failed with external payment provider");
        }
    }
}
