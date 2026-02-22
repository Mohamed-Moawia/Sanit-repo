using StationeryStore.Domain.Entities;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Application.DTOs;

/// <summary>
/// Customer DTO for API responses
/// </summary>
public record CustomerDto(
    Guid Id,
    string NameAr,
    string NameEn,
    CustomerType Type,
    string? TaxRegistrationNumber,
    string? Email,
    string? Phone,
    string? Mobile,
    string? AddressAr,
    string? GovernorateAr,
    string? CityAr,
    decimal CreditLimit,
    decimal CurrentBalance,
    decimal DiscountPercentage,
    decimal LoyaltyPoints,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Customer detail DTO with full information
/// </summary>
public record CustomerDetailDto(
    Guid Id,
    string NameAr,
    string NameEn,
    CustomerType Type,
    string? TaxRegistrationNumber,
    string? TaxCardNumber,
    string? CommercialRegistrationNumber,
    string? EtaReceiverId,
    string? EtaReceiverVatNumber,
    string? Email,
    string? Phone,
    string? Mobile,
    string? Fax,
    string? Website,
    string? AddressAr,
    string? AddressEn,
    string? GovernorateAr,
    string? GovernorateEn,
    string? CityAr,
    string? CityEn,
    string? PostalCode,
    decimal CreditLimit,
    decimal CurrentBalance,
    decimal DiscountPercentage,
    decimal LoyaltyPoints,
    bool IsActive,
    List<CustomerAddressDto> Addresses,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Customer address DTO
/// </summary>
public record CustomerAddressDto(
    Guid Id,
    Guid CustomerId,
    AddressType Type,
    string NameAr,
    string NameEn,
    string AddressAr,
    string AddressEn,
    string GovernorateAr,
    string GovernorateEn,
    string CityAr,
    string CityEn,
    string? DistrictAr,
    string? DistrictEn,
    string PostalCode,
    string Phone,
    string? Email,
    string? Notes,
    bool IsDefault
);

/// <summary>
/// Request to create a customer
/// </summary>
public record CreateCustomerRequest(
    string NameAr,
    string NameEn,
    CustomerType Type,
    string? TaxRegistrationNumber,
    string? TaxCardNumber,
    string? CommercialRegistrationNumber,
    string? Email,
    string? Phone,
    string? Mobile,
    string? Fax,
    string? Website,
    string? AddressAr,
    string? AddressEn,
    string? GovernorateAr,
    string? GovernorateEn,
    string? CityAr,
    string? CityEn,
    string? PostalCode,
    decimal CreditLimit = 0,
    decimal DiscountPercentage = 0,
    string? EtaReceiverId = null,
    string? EtaReceiverVatNumber = null
);

/// <summary>
/// Request to update a customer
/// </summary>
public record UpdateCustomerRequest(
    string NameAr,
    string NameEn,
    CustomerType Type,
    string? TaxRegistrationNumber,
    string? TaxCardNumber,
    string? CommercialRegistrationNumber,
    string? Email,
    string? Phone,
    string? Mobile,
    string? Fax,
    string? Website,
    string? AddressAr,
    string? AddressEn,
    string? GovernorateAr,
    string? GovernorateEn,
    string? CityAr,
    string? CityEn,
    string? PostalCode,
    decimal CreditLimit,
    decimal DiscountPercentage,
    bool IsActive,
    string? EtaReceiverId,
    string? EtaReceiverVatNumber
);

/// <summary>
/// Customer search parameters
/// </summary>
public record CustomerSearchParameters(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    CustomerType? Type = null,
    string? TaxNumber = null,
    bool? IsActive = null
);
