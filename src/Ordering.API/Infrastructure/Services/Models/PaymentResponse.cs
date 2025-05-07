namespace eShop.Ordering.API.Infrastructure.Services.Models;

/// <summary>
/// Ödeme işlemi sonuç modelidir.
/// </summary>
public class PaymentResponse
{
    /// <summary>
    /// Ödeme işlemi başarılı mı?
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Stripe'tan dönen işlem ID'si
    /// </summary>
    public string TransactionId { get; set; }
    
    /// <summary>
    /// İşlem tarihi
    /// </summary>
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Başarısız olursa hata mesajı
    /// </summary>
    public string ErrorMessage { get; set; }
    
    /// <summary>
    /// Ödeme yöntemi
    /// </summary>
    public string PaymentMethod { get; set; }
    
    /// <summary>
    /// İşlem tutarı
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// İşlem para birimi
    /// </summary>
    public string Currency { get; set; }
    
    /// <summary>
    /// Başarılı ödeme için yanıt oluşturur
    /// </summary>
    public static PaymentResponse FromSuccessfulCharge(Stripe.Charge charge)
    {
        return new PaymentResponse
        {
            IsSuccessful = true,
            TransactionId = charge.Id,
            TransactionDate = DateTime.UtcNow,
            PaymentMethod = charge.PaymentMethodDetails?.Type,
            Amount = charge.Amount / 100m, // Stripe tutarları cent cinsinden tutar
            Currency = charge.Currency
        };
    }
    
    /// <summary>
    /// Başarısız ödeme için yanıt oluşturur
    /// </summary>
    public static PaymentResponse FromError(string errorMessage)
    {
        return new PaymentResponse
        {
            IsSuccessful = false,
            TransactionDate = DateTime.UtcNow,
            ErrorMessage = errorMessage
        };
    }
} 