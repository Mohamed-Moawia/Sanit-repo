using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Product category with hierarchical support
/// </summary>
public class Category : LookupEntity
{
    public Guid? ParentCategoryId { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Unit of measure with conversion support
/// </summary>
public class Unit : LookupEntity
{
    public decimal ConversionFactor { get; set; } = 1.0m;
    public string? Symbol { get; set; }
    
    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// VAT rate configuration for Egyptian tax system
/// </summary>
public class VatRate : LookupEntity
{
    public decimal Rate { get; set; }
    public string? EtaTaxTypeCode { get; set; }  // T1=Standard, T2=Exempt
    
    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
