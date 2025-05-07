namespace eShop.Ordering.API.Infrastructure.Services;

using eShop.Ordering.API.Infrastructure.Services.Models;
using Stripe;
using System.Globalization;

/// <summary>
/// Stripe ödeme servisi.
/// </summary>
public class StripePaymentService : IStripePaymentService
{
    private readonly ILogger<StripePaymentService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Stripe ödeme servisi constructor
    /// </summary>
    public StripePaymentService(ILogger<StripePaymentService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Stripe API key'i ayarla
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    /// <summary>
    /// Stripe üzerinden ödeme işlemini gerçekleştirir
    /// </summary>
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Processing Stripe payment for buyer {BuyerId}, amount: {Amount}", 
                request.BuyerId, request.Amount);

            // Stripe token oluştur (gerçek uygulamada bu client tarafında yapılır)
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = request.CardNumber,
                    ExpMonth = long.Parse(request.CardExpiration.Split('/')[0]),
                    ExpYear = long.Parse("20" + request.CardExpiration.Split('/')[1]), // 23 -> 2023
                    Cvc = request.CardSecurityNumber,
                    Name = request.CardHolderName,
                }
            };

            var tokenService = new TokenService();
            var token = await tokenService.CreateAsync(tokenOptions);

            // Ödeme tutarını cent cinsine çevir (Stripe cent cinsinden çalışır)
            var amountInCents = (long)(request.Amount * 100);

            // Ödeme yap
            var options = new ChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = request.Currency,
                Description = request.Description ?? $"Order #{request.OrderId}",
                Source = token.Id,
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", request.OrderId.ToString() },
                    { "BuyerId", request.BuyerId }
                },
                ReceiptEmail = null // Gerçek bir uygulamada burada müşteri email'i olabilir
            };

            var service = new ChargeService();
            var charge = await service.CreateAsync(options);

            // İşlem sonucunu analiz et
            if (charge.Status == "succeeded")
            {
                _logger.LogInformation("Stripe payment succeeded. TransactionId: {TransactionId}, Amount: {Amount}", 
                    charge.Id, charge.Amount / 100m);
                return PaymentResponse.FromSuccessfulCharge(charge);
            }
            else
            {
                _logger.LogWarning("Stripe payment failed. Status: {Status}", charge.Status);
                return PaymentResponse.FromError($"Payment failed with status: {charge.Status}");
            }
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error: {Message}", ex.Message);
            return PaymentResponse.FromError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return PaymentResponse.FromError("An unexpected error occurred while processing the payment.");
        }
    }
} 