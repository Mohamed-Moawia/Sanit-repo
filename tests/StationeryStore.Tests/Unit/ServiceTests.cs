using FluentAssertions;
using Moq;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;
using StationeryStore.Infrastructure.Services;

namespace StationeryStore.Tests.Unit;

/// <summary>
/// Unit tests for Egyptian VAT service
/// </summary>
public class EgyptianVatServiceTests
{
    private readonly EgyptianVatService _vatService;
    
    public EgyptianVatServiceTests()
    {
        var settings = Microsoft.Extensions.Options.Options.Create(new EgyptSettings
        {
            DefaultVatRate = 14.0m
        });
        _vatService = new EgyptianVatService(settings);
    }
    
    [Fact]
    public void CalculateVat_WithStandardRate_ReturnsCorrectAmount()
    {
        // Arrange
        var amount = 100.00m;
        var expectedVat = 14.000m;
        
        // Act
        var result = _vatService.CalculateVat(amount);
        
        // Assert
        result.Should().Be(expectedVat);
    }
    
    [Fact]
    public void CalculateVat_WithCustomRate_ReturnsCorrectAmount()
    {
        // Arrange
        var amount = 100.00m;
        var rate = 10.0m;
        var expectedVat = 10.000m;
        
        // Act
        var result = _vatService.CalculateVat(amount, rate);
        
        // Assert
        result.Should().Be(expectedVat);
    }
    
    [Fact]
    public void CalculateVatExclusive_WithVatInclusiveAmount_ReturnsExclusiveAmount()
    {
        // Arrange
        var vatInclusiveAmount = 114.00m;
        var expectedExclusive = 100.000m;
        
        // Act
        var result = _vatService.CalculateVatExclusive(vatInclusiveAmount);
        
        // Assert
        result.Should().Be(expectedExclusive);
    }
    
    [Fact]
    public void IsValidVatRate_WithValidRates_ReturnsTrue()
    {
        // Act & Assert
        _vatService.IsValidVatRate(0.0m).Should().BeTrue();
        _vatService.IsValidVatRate(14.0m).Should().BeTrue();
    }
    
    [Fact]
    public void IsValidVatRate_WithInvalidRate_ReturnsFalse()
    {
        // Act & Assert
        _vatService.IsValidVatRate(5.0m).Should().BeFalse();
    }
}

/// <summary>
/// Unit tests for Egyptian tax number validator
/// </summary>
public class EgyptianTaxNumberValidatorTests
{
    private readonly EgyptianTaxNumberValidator _validator;
    
    public EgyptianTaxNumberValidatorTests()
    {
        _validator = new EgyptianTaxNumberValidator();
    }
    
    [Theory]
    [InlineData("123-456-789")]
    [InlineData("123456789")]
    [InlineData("987-654-321")]
    public void IsValidTaxRegistrationNumber_WithValidFormat_ReturnsTrue(string taxNumber)
    {
        // Act
        var result = _validator.IsValidTaxRegistrationNumber(taxNumber);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("12-456-789")]
    [InlineData("123-45-789")]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("abc-def-ghi")]
    [InlineData(null)]
    [InlineData("")]
    public void IsValidTaxRegistrationNumber_WithInvalidFormat_ReturnsFalse(string? taxNumber)
    {
        // Act
        var result = _validator.IsValidTaxRegistrationNumber(taxNumber);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("12345678901234")]
    [InlineData("123456789012345")]
    public void IsValidTaxCardNumber_WithValidFormat_ReturnsTrue(string taxCardNumber)
    {
        // Act
        var result = _validator.IsValidTaxCardNumber(taxCardNumber);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ValidateTaxRegistrationNumber_WithValidNumber_ReturnsSuccess()
    {
        // Act
        var result = _validator.ValidateTaxRegistrationNumber("123-456-789");
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ValidateTaxRegistrationNumber_WithInvalidNumber_ReturnsError()
    {
        // Act
        var result = _validator.ValidateTaxRegistrationNumber("invalid");
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeEmpty();
    }
}

/// <summary>
/// Unit tests for Egyptian business hours service
/// </summary>
public class EgyptianBusinessHoursTests
{
    private readonly EgyptianBusinessHours _businessHours;
    
    public EgyptianBusinessHoursTests()
    {
        _businessHours = new EgyptianBusinessHours();
    }
    
    [Theory]
    [InlineData(DayOfWeek.Sunday, true)]
    [InlineData(DayOfWeek.Monday, true)]
    [InlineData(DayOfWeek.Tuesday, true)]
    [InlineData(DayOfWeek.Wednesday, true)]
    [InlineData(DayOfWeek.Thursday, true)]
    [InlineData(DayOfWeek.Friday, false)]
    [InlineData(DayOfWeek.Saturday, false)]
    public void IsBusinessDay_WithVariousDays_ReturnsExpectedResult(DayOfWeek day, bool expected)
    {
        // Act
        var result = _businessHours.IsBusinessDay(day);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void IsBusinessHours_DuringBusinessHours_ReturnsTrue()
    {
        // Arrange
        var dateTime = new DateTime(2026, 2, 18, 10, 0, 0);  // Sunday 10:00 AM
        
        // Act
        var result = _businessHours.IsBusinessHours(dateTime);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsBusinessHours_OutsideBusinessHours_ReturnsFalse()
    {
        // Arrange
        var dateTime = new DateTime(2026, 2, 18, 23, 0, 0);  // Sunday 11:00 PM
        
        // Act
        var result = _businessHours.IsBusinessHours(dateTime);
        
        // Assert
        result.Should().BeFalse();
    }
}
