namespace eShop.Ordering.API.Application.Commands;

public class ProcessOrderPaymentCommand : IRequest<bool>
{
    public int OrderId { get; }
    public string BuyerId { get; }
    public decimal Amount { get; }
    public string CardNumber { get; }
    public string CardHolderName { get; }
    public DateTime CardExpiration { get; }
    public string CardSecurityNumber { get; }

    public ProcessOrderPaymentCommand(int orderId, string buyerId, decimal amount, string cardNumber, 
        string cardHolderName, DateTime cardExpiration, string cardSecurityNumber)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        Amount = amount;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
    }
} 