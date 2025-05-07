namespace eShop.Ordering.API.Infrastructure.Services.Models;

/// <summary>
/// Ödeme işlemi için istek modelidir.
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// Alıcı ID
    /// </summary>
    public string BuyerId { get; set; }
    
    /// <summary>
    /// Ödeme tutarı
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Kart numarası
    /// </summary>
    public string CardNumber { get; set; }
    
    /// <summary>
    /// Kart sahibinin adı
    /// </summary>
    public string CardHolderName { get; set; }
    
    /// <summary>
    /// Kartın son kullanma tarihi (MM/YY formatında)
    /// </summary>
    public string CardExpiration { get; set; }
    
    /// <summary>
    /// Kartın güvenlik numarası (CVV/CVC)
    /// </summary>
    public string CardSecurityNumber { get; set; }
    
    /// <summary>
    /// Sipariş açıklaması (Stripe description alanı için)
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Sipariş numarası (Stripe metadata olarak kaydedilir)
    /// </summary>
    public int OrderId { get; set; }
    
    /// <summary>
    /// İşlem para birimi
    /// </summary>
    public string Currency { get; set; } = "usd";
} 