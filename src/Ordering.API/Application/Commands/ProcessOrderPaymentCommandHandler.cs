namespace eShop.Ordering.API.Application.Commands;

using eShop.Ordering.API.Infrastructure.Services;
using eShop.Ordering.API.Application.IntegrationEvents;
using eShop.Ordering.API.Application.IntegrationEvents.Events;

public class ProcessOrderPaymentCommandHandler : IRequestHandler<ProcessOrderPaymentCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
    private readonly ILogger<ProcessOrderPaymentCommandHandler> _logger;

    public ProcessOrderPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IOrderingIntegrationEventService orderingIntegrationEventService,
        ILogger<ProcessOrderPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling command: {CommandName} - OrderId: {OrderId}", command.GetType().Name, command.OrderId);

        var order = await _orderRepository.GetAsync(command.OrderId);
        
        if (order == null)
        {
            _logger.LogError("Order with Id {OrderId} not found", command.OrderId);
            return false;
        }

        var paymentSucceeded = await _paymentService.ProcessPaymentAsync(
            command.BuyerId, 
            command.Amount, 
            command.CardNumber, 
            command.CardHolderName, 
            command.CardExpiration, 
            command.CardSecurityNumber);

        if (paymentSucceeded)
        {
            _logger.LogInformation("Payment for order {OrderId} processed successfully", command.OrderId);
            
            // Ödeme başarılı eventi yayınla
            var orderPaymentSucceededEvent = new OrderPaymentSucceededIntegrationEvent(command.OrderId);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(orderPaymentSucceededEvent);
            
            // Ödeme başarılı olduğunda sipariş durumunu güncelle (bu senaryoda stok onayından sonraki adım)
            order.SetPaidStatus();
            
            // Değişiklikleri kaydet
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            
            // Eventi yayınla
            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderPaymentSucceededEvent.Id);
            
            return true;
        }
        else
        {
            _logger.LogWarning("Payment for order {OrderId} failed", command.OrderId);
            
            // Ödeme başarısız eventi yayınla
            var orderPaymentFailedEvent = new OrderPaymentFailedIntegrationEvent(command.OrderId);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(orderPaymentFailedEvent);
            
            // Değişiklikleri kaydet (sipariş durumu değişmedi, sadece event kaydedildi)
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            
            // Eventi yayınla
            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderPaymentFailedEvent.Id);
            
            return false;
        }
    }
} 