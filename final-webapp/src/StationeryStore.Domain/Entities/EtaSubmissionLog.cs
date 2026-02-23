using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// ETA e-Invoicing submission log for audit and reconciliation
/// </summary>
public class EtaSubmissionLog : BaseEntity
{
    public Guid InvoiceId { get; set; }
    
    /// <summary>
    /// Submission details
    /// </summary>
    public string SubmissionId { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public EtaSubmissionStatus Status { get; set; }
    
    /// <summary>
    /// ETA response
    /// </summary>
    public string? EtaUuid { get; set; }
    public string? EtaInvoiceTypeCode { get; set; }
    public string? EtaResponseMessage { get; set; }
    public string? EtaRejectionReason { get; set; }
    public string? EtaRejectionCode { get; set; }
    
    /// <summary>
    /// Request/Response tracking
    /// </summary>
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public int HttpStatusCode { get; set; }
    public TimeSpan ResponseTime { get; set; }
    
    /// <summary>
    /// Retry tracking
    /// </summary>
    public int RetryCount { get; set; }
    public DateTime? NextRetryDate { get; set; }
    public string? LastError { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}
