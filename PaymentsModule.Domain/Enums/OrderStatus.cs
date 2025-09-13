namespace PaymentsModule.Domain.Enums;

public enum OrderStatus
{
    PENDING,
    AWAITING_PAYMENT,
    PAID,
    FAILED,
    CANCELED
}
