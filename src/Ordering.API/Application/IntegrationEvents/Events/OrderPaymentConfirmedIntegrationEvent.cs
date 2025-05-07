namespace eShop.Ordering.API.Application.IntegrationEvents.Events;

public record OrderPaymentConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string PaymentTransactionId { get; }
    public DateTime PaymentDate { get; }

    public OrderPaymentConfirmedIntegrationEvent(int orderId, string paymentTransactionId)
    {
        OrderId = orderId;
        PaymentTransactionId = paymentTransactionId;
        PaymentDate = DateTime.UtcNow;
    }
} 