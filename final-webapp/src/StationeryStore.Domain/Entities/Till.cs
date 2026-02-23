using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// POS Till/Cash register entity
/// </summary>
public class Till : BaseEntity
{
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    
    /// <summary>
    /// Cash management
    /// </summary>
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    
    /// <summary>
    /// Till status
    /// </summary>
    public TillStatus Status { get; set; } = TillStatus.Open;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<PosSession> Sessions { get; set; } = new List<PosSession>();
}

/// <summary>
/// POS Session for cashier shift tracking
/// </summary>
public class PosSession : BaseEntity
{
    public Guid TillId { get; set; }
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Session timing
    /// </summary>
    public DateTime SessionStart { get; set; }
    public DateTime? SessionEnd { get; set; }
    
    /// <summary>
    /// Cash tracking
    /// </summary>
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedClosingBalance { get; set; }
    public decimal? Variance => ClosingBalance.HasValue && ExpectedClosingBalance.HasValue 
        ? ClosingBalance.Value - ExpectedClosingBalance.Value 
        : null;
    
    /// <summary>
    /// Session summary
    /// </summary>
    public int TotalTransactions { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCard { get; set; }
    public decimal TotalOther { get; set; }
    
    /// <summary>
    /// Session status
    /// </summary>
    public SessionStatus Status { get; set; } = SessionStatus.Open;
    public string? Notes { get; set; }
    public string? ClosingNotes { get; set; }
    
    // Navigation properties
    public virtual Till Till { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum TillStatus
{
    Open = 0,
    Closed = 1,
    Suspended = 2
}

public enum SessionStatus
{
    Open = 0,
    Closed = 1,
    Suspended = 2
}
