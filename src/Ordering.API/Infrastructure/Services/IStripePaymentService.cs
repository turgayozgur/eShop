namespace eShop.Ordering.API.Infrastructure.Services;

using eShop.Ordering.API.Infrastructure.Services.Models;

/// <summary>
/// Stripe ödeme servisi arayüzü.
/// </summary>
public interface IStripePaymentService
{
    /// <summary>
    /// Stripe üzerinden ödeme işlemini gerçekleştirir
    /// </summary>
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
} 