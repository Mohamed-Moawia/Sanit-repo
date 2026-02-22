using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using Npgsql;
using Serilog;
using Serilog.Events;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;
using StationeryStore.Infrastructure.Caching;
using StationeryStore.Infrastructure.Data;
using StationeryStore.Infrastructure.Data.Interceptors;
using StationeryStore.Infrastructure.Data.Repositories;
using StationeryStore.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

// ============================================================================
// CONFIGURE SERILOG
// ============================================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================================================
// CONFIGURE SERVICES
// ============================================================================

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = environment.IsDevelopment();
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configure Egyptian culture
var egyptianCulture = new CultureInfo("ar-EG")
{
    NumberFormat =
    {
        CurrencySymbol = "ج.م",
        CurrencyDecimalDigits = 3,
        CurrencyDecimalSeparator = ".",
        CurrencyGroupSeparator = ",",
        NumberDecimalDigits = 3,
        NumberDecimalSeparator = ".",
        NumberGroupSeparator = ","
    },
    DateTimeFormat =
    {
        ShortDatePattern = "dd/MM/yyyy",
        LongDatePattern = "dd MMMM yyyy",
        ShortTimePattern = "hh:mm tt",
        LongTimePattern = "hh:mm:ss tt",
        Calendar = new GregorianCalendar()
    }
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        egyptianCulture,
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
});

// Configure PostgreSQL with retry policy
builder.Services.AddDbContext<StoreDbContext>((sp, options) =>
{
    var connectionString = configuration.GetConnectionString("PostgreSQL");
    var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        // NodaTime support is configured via NpgsqlDataSourceBuilder in newer versions
    });

    options.AddInterceptors(auditInterceptor);

    if (environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure Redis for caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "StationeryStore:";
});

// Configure JWT Authentication
var jwtSettings = configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is required"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("User {UserName} authenticated", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("CashierOrAbove", policy => policy.RequireRole("Admin", "Manager", "Cashier"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://*.stationery.eg")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});

// ============================================================================
// REGISTER SERVICES
// ============================================================================

// Settings
builder.Services.Configure<EgyptSettings>(configuration.GetSection("EgyptSettings"));
builder.Services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
builder.Services.Configure<EtaSettings>(configuration.GetSection("EtaSettings"));

// Interceptors
builder.Services.AddScoped<AuditInterceptor>();

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ISpecificationRepository<>), typeof(SpecificationRepository<>));

// Services
builder.Services.AddScoped<IEgyptianVatService, EgyptianVatService>();
builder.Services.AddSingleton<IEgyptianTaxNumberValidator, EgyptianTaxNumberValidator>();
builder.Services.AddSingleton<IEgyptianBusinessHours, EgyptianBusinessHours>();
builder.Services.AddScoped<IEtaEInvoiceService, EtaEInvoiceService>();
builder.Services.AddScoped<ILocalizationService, ArabicEnglishLocalizationService>();
builder.Services.AddSingleton<ICurrencyFormatter, EgyptianCurrencyFormatter>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICacheService, CacheService>();

// Current user provider
builder.Services.AddScoped<ICurrentUserProvider>(sp =>
{
    var httpContext = sp.GetService<IHttpContextAccessor>()?.HttpContext;
    return new CurrentUserProvider(httpContext);
});

// HTTP Client for ETA
builder.Services.AddHttpClient<IEtaEInvoiceService, EtaEInvoiceService>((sp, client) =>
{
    var etaSettings = sp.GetRequiredService<IOptions<EtaSettings>>().Value;
    client.BaseAddress = new Uri(etaSettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(etaSettings.TimeoutInSeconds);
});

// Health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(configuration.GetConnectionString("PostgreSQL") ?? "")
    .AddRedis(configuration.GetConnectionString("Redis") ?? "localhost:6379");

var app = builder.Build();

// ============================================================================
// CONFIGURE PIPELINE
// ============================================================================

// Use Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
});

// Configure pipeline
app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Egypt info endpoint
app.MapGet("/egypt/info", () =>
{
    var egyptSettings = app.Services.GetRequiredService<IOptions<EgyptSettings>>().Value;
    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    
    return Results.Ok(new
    {
        Country = "Egypt",
        Currency = "EGP",
        DefaultVatRate = egyptSettings.DefaultVatRate,
        TimeZone = timeZone.Id,
        CurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone),
        SupportedLanguages = egyptSettings.SupportedLanguages,
        TaxAuthority = "Egyptian Tax Authority (ETA)",
        EInvoiceMandatory = true
    });
}).WithTags("Egypt").WithName("GetEgyptInfo");

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error applying database migrations");
        throw;
    }
}

try
{
    Log.Information("Starting Stationery Store API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for testing
public partial class Program { }

/// <summary>
/// Current user provider implementation
/// </summary>
public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly HttpContext? _httpContext;
    
    public CurrentUserProvider(HttpContext? httpContext)
    {
        _httpContext = httpContext;
    }
    
    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
    
    public string? UserName => _httpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
    
    public string? IpAddress => _httpContext?.Connection.RemoteIpAddress?.ToString();
}
