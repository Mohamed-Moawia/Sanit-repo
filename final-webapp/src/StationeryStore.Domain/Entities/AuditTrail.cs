using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// Audit trail for tracking all data changes
/// </summary>
public class AuditTrail : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public AuditAction Action { get; set; }
    
    /// <summary>
    /// Change tracking
    /// </summary>
    public string? OldValues { get; set; }  // JSON
    public string? NewValues { get; set; }  // JSON
    public string? AffectedColumns { get; set; }  // JSON array
    
    /// <summary>
    /// Context information
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    
    // Navigation properties (if needed)
    public virtual User? User { get; set; }
}

public enum AuditAction
{
    Create = 0,
    Update = 1,
    Delete = 2,
    SoftDelete = 3,
    Restore = 4
}
