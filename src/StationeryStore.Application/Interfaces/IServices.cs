using StationeryStore.Domain.Entities;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Application.Interfaces;

/// <summary>
/// Service interface for Egyptian VAT calculations
/// </summary>
public interface IEgyptianVatService
{
    decimal StandardVatRate { get; }
    decimal CalculateVat(decimal amount, decimal? vatRate = null);
    decimal CalculateVatInclusive(decimal amountIncludingVat, decimal? vatRate = null);
    decimal CalculateVatExclusive(decimal amountIncludingVat, decimal? vatRate = null);
    bool IsValidVatRate(decimal vatRate);
}

/// <summary>
/// Service interface for Egyptian tax number validation
/// </summary>
public interface IEgyptianTaxNumberValidator
{
    bool IsValidTaxRegistrationNumber(string? taxNumber);
    bool IsValidTaxCardNumber(string? taxCardNumber);
    ValidationResult ValidateTaxRegistrationNumber(string? taxNumber);
    ValidationResult ValidateTaxCardNumber(string? taxCardNumber);
}

public record ValidationResult(bool IsValid, string ErrorMessage);

/// <summary>
/// Service interface for Egyptian business hours
/// </summary>
public interface IEgyptianBusinessHours
{
    bool IsBusinessHours(DateTime dateTime);
    bool IsBusinessDay(DayOfWeek dayOfWeek);
    DateTime NextBusinessDay(DateTime dateTime);
    DateTime NextBusinessHours(DateTime dateTime);
    TimeSpan GetBusinessHoursForDay(DayOfWeek dayOfWeek);
}

/// <summary>
/// Service interface for ETA e-Invoicing
/// </summary>
public interface IEtaEInvoiceService
{
    Task<EtaSubmissionResult> SubmitInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<EtaSubmissionResult> CancelInvoiceAsync(string etaUuid, CancellationToken cancellationToken = default);
    Task<EtaSubmissionStatus> GetSubmissionStatusAsync(string submissionId, CancellationToken cancellationToken = default);
    Task<string> GenerateQrCodeAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<string> GenerateDigitalSignatureAsync(string invoiceData, CancellationToken cancellationToken = default);
}

public record EtaSubmissionResult(
    bool IsSuccess,
    string? EtaUuid,
    string? SubmissionId,
    string? Message,
    string? RejectionReason,
    DateTime SubmissionDate
);

/// <summary>
/// Service interface for localization (Arabic/English)
/// </summary>
public interface ILocalizationService
{
    string GetResource(string key, string? language = null);
    string GetResource(string key, params object[] args);
    T GetLocalizedEnum<T>(string enumValue, string? language = null) where T : struct, Enum;
    bool IsArabic(string? language = null);
}

/// <summary>
/// Service interface for currency formatting
/// </summary>
public interface ICurrencyFormatter
{
    string Format(decimal amount, string? currency = "EGP", string? language = null);
    string FormatWithVat(decimal amount, decimal vatRate = 14.0m, string? language = null);
    decimal Parse(string formattedAmount, string? currency = "EGP");
}

/// <summary>
/// Service interface for audit logging
/// </summary>
public interface IAuditService
{
    Task LogCreateAsync<T>(T entity, string userId, string? ipAddress = null) where T : class;
    Task LogUpdateAsync<T>(T entity, T originalEntity, string userId, string? ipAddress = null) where T : class;
    Task LogDeleteAsync<T>(T entity, string userId, string? ipAddress = null) where T : class;
    Task<IEnumerable<AuditTrail>> GetAuditTrailAsync(Guid recordId, string tableName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for authentication
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    string GenerateJwtToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
}

public record AuthenticationResult(
    Guid UserId,
    string Username,
    string FullNameAr,
    string FullNameEn,
    string Email,
    string Role,
    Guid RoleId,
    Guid? DefaultBranchId,
    string Token,
    string RefreshToken,
    int ExpiresInMinutes,
    string Language
);

/// <summary>
/// Service interface for caching
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
