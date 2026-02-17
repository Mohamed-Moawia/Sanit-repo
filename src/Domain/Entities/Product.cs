// Domain/Entities/Product.cs
namespace SanitaryCeramicCMS.Domain.Entities;

public class Product : IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProductCategory Category { get; private set; }
    public ProductType Type { get; private set; }
    public decimal BasePrice { get; private set; }
    public Dimensions Dimensions { get; private set; }
    public Material Material { get; private set; }
    public string Brand { get; private set; }
    public string ImageUrl { get; private set; }
    public string TechnicalSpecs { get; private set; } // JSON field
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
 
    // Value Objects
    public record Dimensions(decimal Length, decimal Width, decimal Height, string Unit);
    public record Material(string Name, string Finish, string Color);
}    
