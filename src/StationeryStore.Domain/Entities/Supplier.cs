using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Supplier entity for procurement management
/// </summary>
public class Supplier : BaseEntity
{
    /// <summary>
    /// Supplier identification
    /// </summary>
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    
    /// <summary>
    /// Egyptian tax information
    /// </summary>
    public string? TaxRegistrationNumber { get; set; }
    public string? TaxCardNumber { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public string? EtaSupplierId { get; set; }
    
    /// <summary>
    /// Contact information
    /// </summary>
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }
    
    /// <summary>
    /// Address
    /// </summary>
    public string? AddressAr { get; set; }
    public string? AddressEn { get; set; }
    public string? GovernorateAr { get; set; }
    public string? GovernorateEn { get; set; }
    public string? CityAr { get; set; }
    public string? CityEn { get; set; }
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Financial terms
    /// </summary>
    public decimal CreditLimit { get; set; }
    public int PaymentTermsDays { get; set; }  // Net 30, 60, etc.
    public decimal DiscountPercentage { get; set; }
    
    /// <summary>
    /// Supplier status
    /// </summary>
    public bool IsActive { get; set; } = true;
    public bool IsPreferred { get; set; }
    
    /// <summary>
    /// Performance metrics
    /// </summary>
    public decimal? AverageDeliveryDays { get; set; }
    public decimal? QualityRating { get; set; }
    
    // Navigation properties
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}

/// <summary>
/// Purchase order for procurement
/// </summary>
public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    
    public Guid SupplierId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    /// <summary>
    /// Financial details
    /// </summary>
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Notes
    /// </summary>
    public string? NotesAr { get; set; }
    public string? NotesEn { get; set; }
    public string? InternalNotes { get; set; }
    
    // Navigation properties
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}

/// <summary>
/// Purchase order line item
/// </summary>
public class PurchaseOrderLine : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public int LineNumber { get; set; }
    
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
    public string ProductNameEn { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    
    public decimal ReceivedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    
    // Navigation properties
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}

/// <summary>
/// Goods receipt for tracking received items
/// </summary>
public class GoodsReceipt : BaseEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    
    public Guid PurchaseOrderId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid BranchId { get; set; }
    
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Pending;
    
    public string? Notes { get; set; }
    public string? InspectionNotes { get; set; }
    
    // Navigation properties
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<GoodsReceiptLine> Lines { get; set; } = new List<GoodsReceiptLine>();
}

/// <summary>
/// Goods receipt line item
/// </summary>
public class GoodsReceiptLine : BaseEntity
{
    public Guid GoodsReceiptId { get; set; }
    public int LineNumber { get; set; }
    
    public Guid PurchaseOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
    
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public decimal AcceptedQuantity => ReceivedQuantity - RejectedQuantity;
    
    public decimal UnitCost { get; set; }
    public string? RejectionReason { get; set; }
    
    // Navigation properties
    public virtual GoodsReceipt GoodsReceipt { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}

public enum ReceiptStatus
{
    Pending = 0,
    PartiallyReceived = 1,
    Completed = 2,
    Rejected = 3,
    Cancelled = 4
}
