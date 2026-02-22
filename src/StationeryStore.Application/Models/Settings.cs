namespace StationeryStore.Application.Models;

/// <summary>
/// Egypt-specific settings
/// </summary>
public class EgyptSettings
{
    public string CountryCode { get; set; } = "EG";
    public string DefaultLanguage { get; set; } = "ar";
    public string DefaultCurrency { get; set; } = "EGP";
    public decimal DefaultVatRate { get; set; } = 14.0m;
    public string TimeZone { get; set; } = "Africa/Cairo";
    public int DecimalPlaces { get; set; } = 3;
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "hh:mm:ss tt";
    public int BusinessWeekStart { get; set; } = 0;  // Sunday
    public int BusinessWeekEnd { get; set; } = 4;    // Thursday
    public string[] SupportedLanguages { get; set; } = new[] { "ar", "en" };
    public string[] EgyptianHolidays { get; set; } = new[]
    {
        "01-01",  // Coptic Christmas
        "01-07",  // Coptic Christmas (Western)
        "04-25",  // Sinai Liberation Day
        "05-01",  // Labor Day
        "06-30",  // June 30 Revolution
        "07-23",  // July 23 Revolution
        "10-06"   // Armed Forces Day
    };
}

/// <summary>
/// Cache settings
/// </summary>
public class CacheSettings
{
    public bool RedisEnabled { get; set; } = true;
    public int SlidingExpirationInMinutes { get; set; } = 30;
    public int AbsoluteExpirationInMinutes { get; set; } = 120;
    public int SizeLimit { get; set; } = 1024;
}

/// <summary>
/// JWT settings
/// </summary>
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; } = 480;
    public int RefreshTokenExpiryInDays { get; set; } = 7;
}

/// <summary>
/// ETA settings for e-invoicing
/// </summary>
public class EtaSettings
{
    public string BaseUrl { get; set; } = "https://api.preprod.invoicing.eta.gov.eg";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public int TimeoutInSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public string Environment { get; set; } = "Sandbox";
    public string ApiVersion { get; set; } = "1.0";
}
