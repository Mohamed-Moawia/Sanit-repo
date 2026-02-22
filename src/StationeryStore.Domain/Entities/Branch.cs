using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Branch/Store location entity for multi-branch operations
/// </summary>
public class Branch : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string AddressAr { get; set; } = string.Empty;
    public string AddressEn { get; set; } = string.Empty;
    
    /// <summary>
    /// Egyptian address fields
    /// </summary>
    public string GovernorateAr { get; set; } = string.Empty;
    public string GovernorateEn { get; set; } = string.Empty;
    public string CityAr { get; set; } = string.Empty;
    public string CityEn { get; set; } = string.Empty;
    public string? DistrictAr { get; set; }
    public string? DistrictEn { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Egyptian tax information (required for e-invoicing)
    /// </summary>
    public string TaxRegistrationNumber { get; set; } = string.Empty;
    public string? TaxActivityCode { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    
    /// <summary>
    /// Contact information
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Fax { get; set; }
    
    /// <summary>
    /// Branch configuration
    /// </summary>
    public bool IsActive { get; set; } = true;
    public bool IsHeadquarters { get; set; }
    public string? ManagerName { get; set; }
    
    /// <summary>
    /// Working days (Sunday-Thursday for Egypt)
    /// </summary>
    private readonly List<DayOfWeek> _workingDays = new();
    public IReadOnlyCollection<DayOfWeek> WorkingDays => _workingDays.AsReadOnly();
    
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    
    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    
    public void SetWorkingDays(IEnumerable<DayOfWeek> days)
    {
        _workingDays.Clear();
        _workingDays.AddRange(days);
    }
}
