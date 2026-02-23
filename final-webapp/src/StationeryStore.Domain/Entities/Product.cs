using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Product entity for stationery items with Egyptian compliance
/// </summary>
public class Product : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public Guid VatRateId { get; set; }
    
    /// <summary>
    /// Product identification
    /// </summary>
    public string Sku { get; set; } = string.Empty;
    public string? EgyptianBarcode { get; set; }  // GS1 Egypt format
    public string? ManufacturerBarcode { get; set; }
    
    /// <summary>
    /// Bilingual names (required by Egyptian law)
    /// </summary>
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    
    /// <summary>
    /// Pricing (Egyptian VAT considerations)
    /// </summary>
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? RetailPrice { get; set; }
    
    /// <summary>
    /// Inventory tracking
    /// </summary>
    public decimal? StockQuantity { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal MaximumStockLevel { get; set; }
    public decimal ReorderPoint { get; set; }
    public decimal ReorderQuantity { get; set; }
    public DateTime? LastRestocked { get; set; }
    
    /// <summary>
    /// ETA Tax Mapping (Egyptian e-invoicing)
    /// </summary>
    public string? EtaItemCode { get; set; }  // ETA product classification
    public string? EtaUnitCode { get; set; }  // ETA unit code
    
    /// <summary>
    /// Product status
    /// </summary>
    public bool IsActive { get; set; } = true;
    public bool RequiresTaxInvoice { get; set; } = true;
    public bool IsExemptFromVat { get; set; }
    
    /// <summary>
    /// Media and attributes
    /// </summary>
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    private readonly List<string> _images = new();
    public IReadOnlyCollection<string> Images => _images.AsReadOnly();
    
    private readonly Dictionary<string, string> _attributes = new();
    public IReadOnlyDictionary<string, string> Attributes => _attributes.AsReadOnly();
    
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual Unit Unit { get; set; } = null!;
    public virtual VatRate VatRate { get; set; } = null!;
    public virtual ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();
    
    public void AddImage(string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl) && !_images.Contains(imageUrl))
            _images.Add(imageUrl);
    }
    
    public void SetAttribute(string key, string value)
    {
        _attributes[key] = value;
    }
}
