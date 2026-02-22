using StationeryStore.Domain.Enums;

namespace StationeryStore.Application.DTOs;

/// <summary>
/// Invoice DTO for API responses
/// </summary>
public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    InvoiceType Type,
    InvoiceStatus Status,
    Guid BranchId,
    string? BranchName,
    Guid? CustomerId,
    string? CustomerName,
    decimal Subtotal,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal DueAmount,
    PaymentMethod? PaymentMethod,
    string? EtaUuid,
    string? EtaSubmissionStatus,
    DateTime CreatedAt
);

/// <summary>
/// Invoice detail DTO with full information
/// </summary>
public record InvoiceDetailDto(
    Guid Id,
    string InvoiceNumber,
    InvoiceType Type,
    InvoiceStatus Status,
    Guid BranchId,
    string? BranchName,
    string? BranchNameEn,
    Guid? TillId,
    Guid? CustomerId,
    string? CustomerName,
    string? CustomerNameEn,
    string? CustomerTaxNumber,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxableAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal DueAmount,
    PaymentMethod? PaymentMethod,
    string? PaymentReference,
    string? EtaUuid,
    string? EtaSubmissionId,
    DateTime? EtaSubmissionDate,
    string? EtaInvoiceTypeCode,
    string? EtaInvoiceSubtypeCode,
    string? EtaPayerId,
    string? EtaPayerVatNumber,
    string? EtaReceiverId,
    string? EtaReceiverVatNumber,
    string? EtaHash,
    string? EtaQrCode,
    string? EtaRejectionReason,
    string? NotesAr,
    string? NotesEn,
    List<InvoiceLineDto> Lines,
    List<PaymentDto> Payments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Invoice line DTO
/// </summary>
public record InvoiceLineDto(
    Guid Id,
    int LineNumber,
    Guid ProductId,
    string ProductCode,
    string ProductNameAr,
    string ProductNameEn,
    string? EgyptianBarcode,
    decimal Quantity,
    string UnitCode,
    decimal UnitPrice,
    decimal LineDiscountAmount,
    decimal LineDiscountPercentage,
    decimal TaxableAmount,
    decimal TaxRate,
    decimal TaxAmount,
    string? EtaTaxTypeCode,
    decimal Subtotal,
    decimal TotalAmount,
    string? EtaItemCode,
    string? EtaUnitCode
);

/// <summary>
/// Payment DTO
/// </summary>
public record PaymentDto(
    Guid Id,
    string PaymentReference,
    PaymentMethod PaymentMethod,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    DateTime CreatedAt
);

/// <summary>
/// Request to create an invoice
/// </summary>
public record CreateInvoiceRequest(
    InvoiceType Type,
    Guid BranchId,
    Guid? TillId,
    Guid? CustomerId,
    List<CreateInvoiceLineRequest> Lines,
    decimal? DiscountAmount = null,
    PaymentMethod? PaymentMethod = null,
    string? PaymentReference = null,
    decimal? PaidAmount = null,
    string? NotesAr = null,
    string? NotesEn = null
);

/// <summary>
/// Request to create an invoice line
/// </summary>
public record CreateInvoiceLineRequest(
    Guid ProductId,
    decimal Quantity,
    decimal? DiscountAmount = null,
    decimal? DiscountPercentage = null
);

/// <summary>
/// Invoice search parameters
/// </summary>
public record InvoiceSearchParameters(
    int Page = 1,
    int PageSize = 20,
    string? InvoiceNumber = null,
    Guid? CustomerId = null,
    Guid? BranchId = null,
    InvoiceType? Type = null,
    InvoiceStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
);
