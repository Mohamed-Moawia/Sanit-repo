namespace StationeryStore.Application.DTOs;

/// <summary>
/// Product DTO for API responses
/// </summary>
public record ProductDto(
    Guid Id,
    string Sku,
    string NameAr,
    string NameEn,
    string? EgyptianBarcode,
    Guid CategoryId,
    string? CategoryName,
    Guid UnitId,
    string? UnitCode,
    decimal CostPrice,
    decimal SalePrice,
    Guid VatRateId,
    decimal TaxRate,
    decimal? StockQuantity,
    decimal MinimumStockLevel,
    decimal ReorderPoint,
    bool IsActive,
    string? EtaItemCode,
    string? EtaUnitCode,
    DateTime CreatedAt
);

/// <summary>
/// Product detail DTO with full information
/// </summary>
public record ProductDetailDto(
    Guid Id,
    string Sku,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? EgyptianBarcode,
    Guid CategoryId,
    string? CategoryName,
    string? CategoryNameEn,
    Guid UnitId,
    string? UnitCode,
    string? UnitName,
    decimal CostPrice,
    decimal SalePrice,
    decimal? WholesalePrice,
    Guid VatRateId,
    decimal TaxRate,
    string? EtaTaxTypeCode,
    decimal? StockQuantity,
    decimal MinimumStockLevel,
    decimal MaximumStockLevel,
    decimal ReorderPoint,
    decimal ReorderQuantity,
    bool IsActive,
    string? EtaItemCode,
    string? EtaUnitCode,
    Guid BranchId,
    string? BranchName,
    List<string> Images,
    Dictionary<string, string> Attributes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Request to create a product
/// </summary>
public record CreateProductRequest(
    string Sku,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? EgyptianBarcode,
    Guid CategoryId,
    Guid UnitId,
    Guid VatRateId,
    Guid BranchId,
    decimal CostPrice,
    decimal SalePrice,
    decimal? WholesalePrice,
    decimal MinimumStockLevel,
    decimal MaximumStockLevel,
    decimal ReorderPoint,
    decimal ReorderQuantity,
    bool IsActive = true,
    string? EtaItemCode = null,
    string? EtaUnitCode = null,
    List<string>? Images = null,
    Dictionary<string, string>? Attributes = null
);

/// <summary>
/// Request to update a product
/// </summary>
public record UpdateProductRequest(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? EgyptianBarcode,
    Guid CategoryId,
    Guid UnitId,
    Guid VatRateId,
    decimal CostPrice,
    decimal SalePrice,
    decimal? WholesalePrice,
    decimal MinimumStockLevel,
    decimal MaximumStockLevel,
    decimal ReorderPoint,
    decimal ReorderQuantity,
    bool IsActive,
    string? EtaItemCode,
    string? EtaUnitCode,
    List<string>? Images,
    Dictionary<string, string>? Attributes
);

/// <summary>
/// Request to update stock quantity
/// </summary>
public record UpdateStockRequest(decimal Quantity);

/// <summary>
/// Product search parameters
/// </summary>
public record ProductSearchParameters(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Barcode = null,
    Guid? CategoryId = null,
    Guid? BranchId = null,
    bool? LowStock = null,
    bool? IsActive = null
);
