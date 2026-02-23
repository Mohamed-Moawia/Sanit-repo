using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationeryStore.Application.Common;
using StationeryStore.Application.DTOs;
using StationeryStore.Domain.Entities;
using StationeryStore.Domain.Enums;
using StationeryStore.Infrastructure.Data;

namespace StationeryStore.API.Controllers;

/// <summary>
/// Customers management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly ILogger<CustomersController> _logger;
    
    public CustomersController(StoreDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all customers with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<CustomerDto>>>> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] CustomerType? type = null,
        [FromQuery] string? taxNumber = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var query = _context.Customers.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.NameAr.Contains(search) ||
                    c.NameEn.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)) ||
                    (c.Phone != null && c.Phone.Contains(search)));
            }
            
            if (type.HasValue)
            {
                query = query.Where(c => c.Type == type.Value);
            }
            
            if (!string.IsNullOrEmpty(taxNumber))
            {
                query = query.Where(c => c.TaxRegistrationNumber == taxNumber);
            }
            
            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }
            
            var totalItems = await query.CountAsync();
            var customers = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto(
                    c.Id,
                    c.NameAr,
                    c.NameEn,
                    c.Type,
                    c.TaxRegistrationNumber,
                    c.Email,
                    c.Phone,
                    c.Mobile,
                    c.AddressAr,
                    c.GovernorateAr,
                    c.CityAr,
                    c.CreditLimit,
                    c.CurrentBalance,
                    c.DiscountPercentage,
                    c.LoyaltyPoints,
                    c.IsActive,
                    c.CreatedAt))
                .ToListAsync();
            
            var pagedResponse = PagedResponse<CustomerDto>.Create(customers, totalItems, page, pageSize);
            return Ok(ApiResponse<PagedResponse<CustomerDto>>.SuccessResponse(pagedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers");
            return StatusCode(500, ApiResponse<PagedResponse<CustomerDto>>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CustomerDetailDto>>> GetCustomer(Guid id)
    {
        try
        {
            var customer = await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            
            if (customer == null)
                return NotFound(ApiResponse<CustomerDetailDto>.FailureResponse("Customer not found"));
            
            var response = new CustomerDetailDto(
                customer.Id,
                customer.NameAr,
                customer.NameEn,
                customer.Type,
                customer.TaxRegistrationNumber,
                customer.TaxCardNumber,
                customer.CommercialRegistrationNumber,
                customer.EtaReceiverId,
                customer.EtaReceiverVatNumber,
                customer.Email,
                customer.Phone,
                customer.Mobile,
                customer.Fax,
                customer.Website,
                customer.AddressAr,
                customer.AddressEn,
                customer.GovernorateAr,
                customer.GovernorateEn,
                customer.CityAr,
                customer.CityEn,
                customer.PostalCode,
                customer.CreditLimit,
                customer.CurrentBalance,
                customer.DiscountPercentage,
                customer.LoyaltyPoints,
                customer.IsActive,
                customer.Addresses.Select(a => new CustomerAddressDto(
                    a.Id,
                    a.CustomerId,
                    a.Type,
                    a.NameAr,
                    a.NameEn,
                    a.AddressAr,
                    a.AddressEn,
                    a.GovernorateAr,
                    a.GovernorateEn,
                    a.CityAr,
                    a.CityEn,
                    a.DistrictAr,
                    a.DistrictEn,
                    a.PostalCode,
                    a.Phone,
                    a.Email,
                    a.Notes,
                    a.IsDefault)).ToList(),
                customer.CreatedAt,
                customer.UpdatedAt);
            
            return Ok(ApiResponse<CustomerDetailDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CustomerDetailDto>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Create new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerDetailDto>>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = new Customer
            {
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                Type = request.Type,
                TaxRegistrationNumber = request.TaxRegistrationNumber,
                TaxCardNumber = request.TaxCardNumber,
                CommercialRegistrationNumber = request.CommercialRegistrationNumber,
                Email = request.Email,
                Phone = request.Phone,
                Mobile = request.Mobile,
                Fax = request.Fax,
                Website = request.Website,
                AddressAr = request.AddressAr,
                AddressEn = request.AddressEn,
                GovernorateAr = request.GovernorateAr,
                GovernorateEn = request.GovernorateEn,
                CityAr = request.CityAr,
                CityEn = request.CityEn,
                PostalCode = request.PostalCode,
                CreditLimit = request.CreditLimit,
                DiscountPercentage = request.DiscountPercentage,
                EtaReceiverId = request.EtaReceiverId,
                EtaReceiverVatNumber = request.EtaReceiverVatNumber
            };
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Customer {CustomerName} created successfully", customer.NameAr);
            
            var response = new CustomerDetailDto(
                customer.Id,
                customer.NameAr,
                customer.NameEn,
                customer.Type,
                customer.TaxRegistrationNumber,
                customer.TaxCardNumber,
                customer.CommercialRegistrationNumber,
                customer.EtaReceiverId,
                customer.EtaReceiverVatNumber,
                customer.Email,
                customer.Phone,
                customer.Mobile,
                customer.Fax,
                customer.Website,
                customer.AddressAr,
                customer.AddressEn,
                customer.GovernorateAr,
                customer.GovernorateEn,
                customer.CityAr,
                customer.CityEn,
                customer.PostalCode,
                customer.CreditLimit,
                customer.CurrentBalance,
                customer.DiscountPercentage,
                customer.LoyaltyPoints,
                customer.IsActive,
                new List<CustomerAddressDto>(),
                customer.CreatedAt,
                customer.UpdatedAt);
            
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id },
                ApiResponse<CustomerDetailDto>.SuccessResponse(response, "Customer created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, ApiResponse<CustomerDetailDto>.FailureResponse("An error occurred"));
        }
    }
}
