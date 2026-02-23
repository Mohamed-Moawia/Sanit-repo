using Microsoft.Extensions.Options;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;

namespace StationeryStore.Infrastructure.Services;

/// <summary>
/// Egyptian VAT service implementation
/// </summary>
public class EgyptianVatService : IEgyptianVatService
{
    private readonly EgyptSettings _settings;
    
    public EgyptianVatService(IOptions<EgyptSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public decimal StandardVatRate => _settings.DefaultVatRate;
    
    public decimal CalculateVat(decimal amount, decimal? vatRate = null)
    {
        var rate = vatRate ?? _settings.DefaultVatRate;
        return Math.Round(amount * (rate / 100), 3);  // 3 decimals for milliemes
    }
    
    public decimal CalculateVatInclusive(decimal amountIncludingVat, decimal? vatRate = null)
    {
        var rate = vatRate ?? _settings.DefaultVatRate;
        return Math.Round(amountIncludingVat - CalculateVatExclusive(amountIncludingVat, rate), 3);
    }
    
    public decimal CalculateVatExclusive(decimal amountIncludingVat, decimal? vatRate = null)
    {
        var rate = vatRate ?? _settings.DefaultVatRate;
        return Math.Round(amountIncludingVat / (1 + (rate / 100)), 3);
    }
    
    public bool IsValidVatRate(decimal vatRate)
    {
        // Egyptian VAT rates: 0% (exempt) or 14% (standard)
        return vatRate is 0.0m or 14.0m;
    }
}

/// <summary>
/// Egyptian tax number validator
/// </summary>
public class EgyptianTaxNumberValidator : IEgyptianTaxNumberValidator
{
    public bool IsValidTaxRegistrationNumber(string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            return false;
        
        // Egyptian tax registration number format: XXX-XXX-XXX (9 digits)
        var cleaned = taxNumber.Replace("-", "").Replace(" ", "");
        return cleaned.Length == 9 && cleaned.All(char.IsDigit);
    }
    
    public bool IsValidTaxCardNumber(string? taxCardNumber)
    {
        if (string.IsNullOrWhiteSpace(taxCardNumber))
            return false;
        
        // Egyptian tax card number: 14 digits
        var cleaned = taxCardNumber.Replace("-", "").Replace(" ", "");
        return cleaned.Length == 14 && cleaned.All(char.IsDigit);
    }
    
    public ValidationResult ValidateTaxRegistrationNumber(string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber))
            return new ValidationResult(false, "Tax registration number is required / رقم التسجيل الضريبي مطلوب");
        
        var cleaned = taxNumber.Replace("-", "").Replace(" ", "");
        if (cleaned.Length != 9 || !cleaned.All(char.IsDigit))
            return new ValidationResult(false, "Invalid tax registration number format (9 digits required) / تنسيق رقم التسجيل الضريبي غير صحيح (9 أرقام مطلوبة)");
        
        return new ValidationResult(true, string.Empty);
    }
    
    public ValidationResult ValidateTaxCardNumber(string? taxCardNumber)
    {
        if (string.IsNullOrWhiteSpace(taxCardNumber))
            return new ValidationResult(false, "Tax card number is required / رقم البطاقة الضريبية مطلوب");
        
        var cleaned = taxCardNumber.Replace("-", "").Replace(" ", "");
        if (cleaned.Length != 14 || !cleaned.All(char.IsDigit))
            return new ValidationResult(false, "Invalid tax card number format (14 digits required) / تنسيق رقم البطاقة الضريبية غير صحيح (14 رقمًا مطلوبًا)");
        
        return new ValidationResult(true, string.Empty);
    }
}

/// <summary>
/// Egyptian business hours service
/// </summary>
public class EgyptianBusinessHours : IEgyptianBusinessHours
{
    // Egypt business week: Sunday-Thursday
    private static readonly HashSet<DayOfWeek> BusinessDays = new()
    {
        DayOfWeek.Sunday,
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday
    };
    
    private static readonly TimeSpan BusinessStart = new(9, 0, 0);   // 9:00 AM
    private static readonly TimeSpan BusinessEnd = new(22, 0, 0);    // 10:00 PM
    
    public bool IsBusinessHours(DateTime dateTime)
    {
        return IsBusinessDay(dateTime.DayOfWeek) &&
               dateTime.TimeOfDay >= BusinessStart &&
               dateTime.TimeOfDay <= BusinessEnd;
    }
    
    public bool IsBusinessDay(DayOfWeek dayOfWeek)
    {
        return BusinessDays.Contains(dayOfWeek);
    }
    
    public DateTime NextBusinessDay(DateTime dateTime)
    {
        var nextDay = dateTime.AddDays(1);
        while (!IsBusinessDay(nextDay.DayOfWeek))
        {
            nextDay = nextDay.AddDays(1);
        }
        return nextDay.Date.Add(BusinessStart);
    }
    
    public DateTime NextBusinessHours(DateTime dateTime)
    {
        if (IsBusinessDay(dateTime.DayOfWeek))
        {
            if (dateTime.TimeOfDay < BusinessStart)
                return dateTime.Date.Add(BusinessStart);
            
            if (dateTime.TimeOfDay > BusinessEnd)
                return NextBusinessDay(dateTime);
            
            return dateTime;  // Already in business hours
        }
        
        return NextBusinessDay(dateTime);
    }
    
    public TimeSpan GetBusinessHoursForDay(DayOfWeek dayOfWeek)
    {
        return IsBusinessDay(dayOfWeek) ? BusinessEnd - BusinessStart : TimeSpan.Zero;
    }
}
