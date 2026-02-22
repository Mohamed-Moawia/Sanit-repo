using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Inventory stock tracking per product per branch
/// </summary>
public class InventoryStock : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? LocationId { get; set; }
    
    /// <summary>
    /// Stock quantities
    /// </summary>
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;
    public decimal QuantityInTransit { get; set; }
    
    /// <summary>
    /// Cost tracking
    /// </summary>
    public decimal AverageCost { get; set; }
    public decimal LastPurchaseCost { get; set; }
    
    /// <summary>
    /// Reorder configuration
    /// </summary>
    public decimal ReorderPoint { get; set; }
    public decimal ReorderQuantity { get; set; }
    public bool IsLowStock => QuantityAvailable <= ReorderPoint;
    
    /// <summary>
    /// Stock count tracking
    /// </summary>
    public DateTime? LastCountDate { get; set; }
    public decimal? LastCountQuantity { get; set; }
    
    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual InventoryLocation? Location { get; set; }
    public virtual ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}

/// <summary>
/// Inventory location (warehouse, shelf, bin)
/// </summary>
public class InventoryLocation : BaseEntity
{
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public LocationType Type { get; set; }
    
    /// <summary>
    /// Location hierarchy
    /// </summary>
    public string? WarehouseCode { get; set; }
    public string? AisleCode { get; set; }
    public string? RackCode { get; set; }
    public string? ShelfCode { get; set; }
    public string? BinCode { get; set; }
    
    /// <summary>
    /// Capacity
    /// </summary>
    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentOccupancy { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<InventoryStock> Stocks { get; set; } = new List<InventoryStock>();
}

/// <summary>
/// Inventory movement audit trail
/// </summary>
public class InventoryMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? StockId { get; set; }
    public Guid? LocationId { get; set; }
    
    public MovementType MovementType { get; set; }
    public decimal Quantity { get; set; }  // Positive=in, Negative=out
    public decimal UnitCost { get; set; }
    public decimal TotalCost => Quantity * UnitCost;
    
    /// <summary>
    /// Reference information
    /// </summary>
    public string? ReferenceType { get; set; }  // "Invoice", "PurchaseOrder", "Adjustment"
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    
    /// <summary>
    /// Additional info
    /// </summary>
    public string? Notes { get; set; }
    public string? PerformedBy { get; set; }
    
    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual InventoryStock? Stock { get; set; }
    public virtual InventoryLocation? Location { get; set; }
}

public enum LocationType
{
    Warehouse = 0,
    Shelf = 1,
    Bin = 2,
    Display = 3,
    BackStock = 4
}
