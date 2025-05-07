namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentConfirmedIntegrationEventHandler(
    IMediator mediator,
    ILogger<OrderPaymentConfirmedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<OrderPaymentConfirmedIntegrationEvent>
{
    public async Task Handle(OrderPaymentConfirmedIntegrationEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        // Bu event'i aldığımızda, ödeme tamamlanmış demektir. Sipariş durumunu güncelleme komutu gönderilebilir.
        var command = new SetPaidOrderStatusCommand(@event.OrderId);

        logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await mediator.Send(command);
    }
} 