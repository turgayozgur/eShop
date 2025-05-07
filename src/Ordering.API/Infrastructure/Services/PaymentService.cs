namespace eShop.Ordering.API.Infrastructure.Services;

using eShop.Ordering.API.Infrastructure.Services.Models;
using Microsoft.Extensions.Http.Resilience;
using System.Net.Http;
using System.Net.Http.Json;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(string buyerId, decimal amount, string cardNumber, 
        string cardHolderName, DateTime cardExpiration, string cardSecurityNumber);
}

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IStripePaymentService _stripePaymentService;

    public PaymentService(
        ILogger<PaymentService> logger, 
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        IStripePaymentService stripePaymentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _stripePaymentService = stripePaymentService ?? throw new ArgumentNullException(nameof(stripePaymentService));
    }

    public async Task<bool> ProcessPaymentAsync(string buyerId, decimal amount, string cardNumber, 
        string cardHolderName, DateTime cardExpiration, string cardSecurityNumber)
    {
        _logger.LogInformation("Processing payment for buyer {BuyerId}, amount: {Amount}", buyerId, amount);

        try
        {
            // Stripe ödeme işlemcisini kullan
            var paymentRequest = new PaymentRequest
            {
                BuyerId = buyerId,
                Amount = amount,
                CardNumber = cardNumber,
                CardHolderName = cardHolderName,
                CardExpiration = cardExpiration.ToString("MM/yy"),
                CardSecurityNumber = cardSecurityNumber,
                Currency = "usd", // Varsayılan para birimi
                Description = $"Payment for order by {buyerId}"
            };

            var paymentResponse = await _stripePaymentService.ProcessPaymentAsync(paymentRequest);

            if (paymentResponse.IsSuccessful)
            {
                _logger.LogInformation("Payment processed successfully. TransactionId: {TransactionId}", 
                    paymentResponse.TransactionId);
                return true;
            }
            else
            {
                _logger.LogWarning("Payment failed: {ErrorMessage}", paymentResponse.ErrorMessage);
                return false;
            }

            // Simülasyon kodu artık gerekli değil, Stripe entegrasyonunu kullanıyoruz.
            // Aşağıdaki kodu simülasyon için test amaçlı saklıyoruz
            
            /*
            // Extensions.cs içinde yapılandırılmış, resilient HTTP client'ı kullan
            var client = _httpClientFactory.CreateClient("PaymentGateway");
            
            // Gerçek bir HTTP isteği yapmak yerine simülasyon yapıyoruz
            // Üretim ortamında burada bir HTTP çağrısı olacak
            return await SimulatePaymentGatewayCallAsync(
                buyerId, amount, cardNumber, cardHolderName, cardExpiration, cardSecurityNumber);
            */
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for buyer {BuyerId}", buyerId);
            return false;
        }
    }

    // Eski simülasyon kodu
    private async Task<bool> SimulatePaymentGatewayCallAsync(string buyerId, decimal amount, string cardNumber, 
        string cardHolderName, DateTime cardExpiration, string cardSecurityNumber)
    {
        // Simülasyon için basit gecikme
        await Task.Delay(TimeSpan.FromMilliseconds(500));

        // Test için %80 başarı oranı simüle ediyoruz
        var random = new Random();
        var isSuccessful = random.Next(100) < 80;

        _logger.LogInformation("Payment for buyer {BuyerId} completed with status {Status}", 
            buyerId, isSuccessful ? "Success" : "Failed");
        
        return isSuccessful;
    }
} 