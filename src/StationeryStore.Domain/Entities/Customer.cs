using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Customer entity with Egyptian tax compliance
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Customer identification
    /// </summary>
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public CustomerType Type { get; set; }
    
    /// <summary>
    /// Egyptian tax information
    /// </summary>
    public string? TaxRegistrationNumber { get; set; }
    public string? TaxCardNumber { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public string? EtaReceiverId { get; set; }  // ETA receiver ID for e-invoicing
    public string? EtaReceiverVatNumber { get; set; }
    
    /// <summary>
    /// Contact information
    /// </summary>
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }
    
    /// <summary>
    /// Address (primary)
    /// </summary>
    public string? AddressAr { get; set; }
    public string? AddressEn { get; set; }
    public string? GovernorateAr { get; set; }
    public string? GovernorateEn { get; set; }
    public string? CityAr { get; set; }
    public string? CityEn { get; set; }
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Financial information
    /// </summary>
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal DiscountPercentage { get; set; }
    
    /// <summary>
    /// Loyalty points
    /// </summary>
    public decimal LoyaltyPoints { get; set; }
    
    /// <summary>
    /// Customer status
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
    
    public void AddLoyaltyPoints(decimal points)
    {
        LoyaltyPoints += points;
    }
    
    public void DeductLoyaltyPoints(decimal points)
    {
        LoyaltyPoints = Math.Max(0, LoyaltyPoints - points);
    }
}

/// <summary>
/// Customer address for multiple shipping/billing addresses
/// </summary>
public class CustomerAddress : BaseEntity
{
    public Guid CustomerId { get; set; }
    public AddressType Type { get; set; }  // Billing, Shipping, Both
    
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string AddressAr { get; set; } = string.Empty;
    public string AddressEn { get; set; } = string.Empty;
    public string GovernorateAr { get; set; } = string.Empty;
    public string GovernorateEn { get; set; } = string.Empty;
    public string CityAr { get; set; } = string.Empty;
    public string CityEn { get; set; } = string.Empty;
    public string? DistrictAr { get; set; }
    public string? DistrictEn { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; }
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}

public enum AddressType
{
    Billing = 0,
    Shipping = 1,
    Both = 2
}

/// <summary>
/// Loyalty transaction tracking
/// </summary>
public class LoyaltyTransaction : BaseEntity
{
    public Guid CustomerId { get; set; }
    public decimal Points { get; set; }  // Positive=earned, Negative=redeemed
    public string? Description { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public Guid? ReferenceId { get; set; }  // Invoice ID or other reference
    public string? ReferenceType { get; set; }  // "Invoice", "Promotion", etc.
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}

public enum LoyaltyTransactionType
{
    Earned = 0,
    Redeemed = 1,
    Adjusted = 2,
    Expired = 3
}
