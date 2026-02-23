using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StationeryStore.Application.Interfaces;
using StationeryStore.Domain.Entities;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Infrastructure.Services;

/// <summary>
/// ETA e-Invoicing service implementation for Egyptian Tax Authority integration
/// </summary>
public class EtaEInvoiceService : IEtaEInvoiceService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EtaEInvoiceService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public EtaEInvoiceService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<EtaEInvoiceService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
    
    public async Task<EtaSubmissionResult> SubmitInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting invoice {InvoiceNumber} to ETA", invoice.InvoiceNumber);
            
            // Prepare invoice payload for ETA API
            var payload = PrepareEtaPayload(invoice);
            var content = new StringContent(
                JsonSerializer.Serialize(payload, _jsonOptions),
                Encoding.UTF8,
                "application/json");
            
            // Add ETA headers
            AddEtaHeaders();
            
            // Submit to ETA API
            var response = await _httpClient.PostAsync("api/v1/documents/invoices", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<EtaResponse>(responseContent, _jsonOptions);
                
                _logger.LogInformation("Invoice {InvoiceNumber} submitted successfully to ETA. UUID: {EtaUuid}",
                    invoice.InvoiceNumber, result?.Uuid);
                
                return new EtaSubmissionResult(
                    true,
                    result?.Uuid,
                    result?.SubmissionId,
                    "Invoice submitted successfully",
                    null,
                    DateTime.UtcNow
                );
            }
            else
            {
                _logger.LogError("Failed to submit invoice {InvoiceNumber} to ETA. Status: {StatusCode}, Response: {Response}",
                    invoice.InvoiceNumber, response.StatusCode, responseContent);
                
                return new EtaSubmissionResult(
                    false,
                    null,
                    null,
                    "Submission failed",
                    responseContent,
                    DateTime.UtcNow
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting invoice {InvoiceNumber} to ETA", invoice.InvoiceNumber);
            
            return new EtaSubmissionResult(
                false,
                null,
                null,
                "Submission error",
                ex.Message,
                DateTime.UtcNow
            );
        }
    }
    
    public async Task<EtaSubmissionResult> CancelInvoiceAsync(
        string etaUuid,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling invoice {EtaUuid} in ETA", etaUuid);
            
            AddEtaHeaders();
            
            var response = await _httpClient.PostAsync(
                $"api/v1/documents/invoices/{etaUuid}/cancel",
                null,
                cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Invoice {EtaUuid} cancelled successfully", etaUuid);
                
                return new EtaSubmissionResult(
                    true,
                    etaUuid,
                    null,
                    "Invoice cancelled successfully",
                    null,
                    DateTime.UtcNow
                );
            }
            
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return new EtaSubmissionResult(
                false,
                etaUuid,
                null,
                "Cancellation failed",
                errorContent,
                DateTime.UtcNow
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling invoice {EtaUuid}", etaUuid);
            
            return new EtaSubmissionResult(
                false,
                etaUuid,
                null,
                "Cancellation error",
                ex.Message,
                DateTime.UtcNow
            );
        }
    }
    
    public async Task<EtaSubmissionStatus> GetSubmissionStatusAsync(
        string submissionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddEtaHeaders();
            
            var response = await _httpClient.GetAsync(
                $"api/v1/documents/invoices/{submissionId}/status",
                cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<EtaStatusResponse>(content, _jsonOptions);
                
                return result?.Status?.ToLowerInvariant() switch
                {
                    "approved" => EtaSubmissionStatus.Approved,
                    "rejected" => EtaSubmissionStatus.Rejected,
                    "pending" => EtaSubmissionStatus.Pending,
                    _ => EtaSubmissionStatus.Submitted
                };
            }
            
            return EtaSubmissionStatus.Pending;
        }
        catch
        {
            return EtaSubmissionStatus.Pending;
        }
    }
    
    public Task<string> GenerateQrCodeAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        // Generate TLV-encoded QR code for Egyptian ETA compliance
        var qrData = GenerateTlvQrData(invoice);
        var qrCodeBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(qrData));
        return Task.FromResult(qrCodeBase64);
    }
    
    public Task<string> GenerateDigitalSignatureAsync(string invoiceData, CancellationToken cancellationToken = default)
    {
        // Placeholder for digital signature implementation
        // In production, this would use the company's digital certificate
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(invoiceData));
        return Task.FromResult(Convert.ToBase64String(hash));
    }
    
    private object PrepareEtaPayload(Invoice invoice)
    {
        return new
        {
            invoiceTypeCode = invoice.EtaInvoiceTypeCode ?? "I",
            invoiceSubtypeCode = invoice.EtaInvoiceSubtypeCode ?? "standard",
            currencyCode = invoice.EtaCurrencyCode ?? "EGP",
            exchangeRate = invoice.EtaExchangeRate ?? "1",
            invoiceNumber = invoice.InvoiceNumber,
            invoiceDate = invoice.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            supplyDate = invoice.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            payer = new
            {
                id = invoice.EtaPayerId,
                vatNumber = invoice.EtaPayerVatNumber
            },
            receiver = new
            {
                id = invoice.EtaReceiverId,
                vatNumber = invoice.EtaReceiverVatNumber
            },
            lines = invoice.Lines?.Select(l => new
            {
                lineNumber = l.LineNumber,
                itemCode = l.EtaItemCode ?? "0",
                quantity = l.Quantity,
                unitPrice = l.UnitPrice,
                taxRate = l.TaxRate,
                taxAmount = l.TaxAmount,
                totalAmount = l.TotalAmount
            }),
            totals = new
            {
                subtotal = invoice.Subtotal,
                taxTotal = invoice.TaxAmount,
                grandTotal = invoice.TotalAmount
            }
        };
    }
    
    private void AddEtaHeaders()
    {
        var etaSettings = _configuration.GetSection("EtaSettings");
        var clientId = etaSettings["ClientId"];
        var clientSecret = etaSettings["ClientSecret"];
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ar");
        _httpClient.DefaultRequestHeaders.Add("X-Request-Id", Guid.NewGuid().ToString());
        
        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Client-Id", clientId);
            _httpClient.DefaultRequestHeaders.Add("X-Client-Secret", clientSecret);
        }
    }
    
    private static string GenerateTlvQrData(Invoice invoice)
    {
        // TLV encoding for Egyptian ETA QR code
        var sb = new StringBuilder();
        sb.Append(invoice.EtaReceiverId ?? string.Empty).Append('\t');  // Seller VAT ID
        sb.Append(invoice.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")).Append('\t');  // Invoice timestamp
        sb.Append(invoice.TotalAmount.ToString("F3")).Append('\t');  // Total amount
        sb.Append(invoice.TaxAmount.ToString("F3"));  // VAT total
        return sb.ToString();
    }
    
    private record EtaResponse(string? Uuid, string? SubmissionId, string? Status);
    private record EtaStatusResponse(string? Status);
}
