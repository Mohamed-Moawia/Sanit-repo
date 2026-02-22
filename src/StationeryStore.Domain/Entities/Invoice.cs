using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Sales invoice with Egyptian ETA e-invoicing support
/// </summary>
public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType Type { get; set; } = InvoiceType.TaxInvoice;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    
    /// <summary>
    /// Branch & Till
    /// </summary>
    public Guid BranchId { get; set; }
    public Guid? TillId { get; set; }
    
    /// <summary>
    /// Customer (optional for walk-in)
    /// </summary>
    public Guid? CustomerId { get; set; }
    
    /// <summary>
    /// Financial Details
    /// </summary>
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    
    /// <summary>
    /// Payment
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    
    /// <summary>
    /// Egyptian E-Invoicing (ETA)
    /// </summary>
    public string? EtaUuid { get; set; }
    public string? EtaSubmissionId { get; set; }
    public DateTime? EtaSubmissionDate { get; set; }
    public string? EtaInvoiceTypeCode { get; set; }  // I=Invoice, D=Debit, P=Payment
    public string? EtaInvoiceSubtypeCode { get; set; }
    public string? EtaCurrencyCode { get; set; } = "EGP";
    public string? EtaExchangeRate { get; set; }
    public string? EtaPayerId { get; set; }  // Customer ETA ID
    public string? EtaPayerVatNumber { get; set; }
    public string? EtaReceiverId { get; set; }  // Seller ETA ID
    public string? EtaReceiverVatNumber { get; set; }
    public string? EtaHash { get; set; }  // Digital signature hash
    public string? EtaQrCode { get; set; }  // QR code for receipt
    public string? EtaRejectionReason { get; set; }
    
    /// <summary>
    /// Notes
    /// </summary>
    public string? NotesAr { get; set; }
    public string? NotesEn { get; set; }
    public string? InternalNotes { get; set; }
    
    // Navigation properties
    public virtual Branch? Branch { get; set; }
    public virtual Till? Till { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<InvoiceLine>? Lines { get; set; } = new List<InvoiceLine>();
    public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    public virtual ICollection<EtaSubmissionLog>? EtaSubmissionLogs { get; set; } = new List<EtaSubmissionLog>();
}

/// <summary>
/// Invoice line item
/// </summary>
public class InvoiceLine : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public int LineNumber { get; set; }
    
    /// <summary>
    /// Product information (snapshot at time of sale)
    /// </summary>
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
    public string ProductNameEn { get; set; } = string.Empty;
    public string? EgyptianBarcode { get; set; }
    
    /// <summary>
    /// Quantity and pricing
    /// </summary>
    public decimal Quantity { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal LineDiscountAmount { get; set; }
    public decimal LineDiscountPercentage { get; set; }
    
    /// <summary>
    /// Tax calculation
    /// </summary>
    public decimal TaxableAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public string? EtaTaxTypeCode { get; set; }
    
    /// <summary>
    /// Totals
    /// </summary>
    public decimal Subtotal { get; set; }
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// ETA mapping
    /// </summary>
    public string? EtaItemCode { get; set; }
    public string? EtaUnitCode { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}

/// <summary>
/// Payment transaction
/// </summary>
public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}
