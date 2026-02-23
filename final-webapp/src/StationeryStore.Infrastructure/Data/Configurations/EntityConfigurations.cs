using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StationeryStore.Domain.Entities;

namespace StationeryStore.Infrastructure.Data.Configurations;

/// <summary>
/// Product entity configuration
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Sku)
            .IsUnique();
        
        builder.Property(p => p.EgyptianBarcode)
            .HasMaxLength(20);
        
        builder.HasIndex(p => p.EgyptianBarcode)
            .IsUnique();
        
        builder.Property(p => p.NameAr)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.NameEn)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.DescriptionAr)
            .HasMaxLength(1000);
        
        builder.Property(p => p.DescriptionEn)
            .HasMaxLength(1000);
        
        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.SalePrice)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.WholesalePrice)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.StockQuantity)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.MinimumStockLevel)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.MaximumStockLevel)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.ReorderPoint)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.ReorderQuantity)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.EtaItemCode)
            .HasMaxLength(50);
        
        builder.Property(p => p.EtaUnitCode)
            .HasMaxLength(10);
        
        // Relationships
        builder.HasOne(p => p.Branch)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Unit)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.VatRate)
            .WithMany(v => v.Products)
            .HasForeignKey(p => p.VatRateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Category entity configuration
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.NameAr)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.NameEn)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Code)
            .HasMaxLength(20);
        
        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(500);
        
        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(500);
        
        builder.HasIndex(c => c.Code)
            .IsUnique();
        
        // Self-referencing relationship for hierarchical categories
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Invoice entity configuration
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();
        
        builder.Property(i => i.EtaUuid)
            .HasMaxLength(100);
        
        builder.HasIndex(i => i.EtaUuid)
            .IsUnique();
        
        builder.Property(i => i.EtaSubmissionId)
            .HasMaxLength(100);
        
        builder.Property(i => i.EtaHash)
            .HasMaxLength(500);
        
        builder.Property(i => i.EtaQrCode)
            .HasMaxLength(2000);
        
        builder.Property(i => i.Subtotal)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.DiscountAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.TaxableAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.TaxAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.TotalAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.PaidAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.DueAmount)
            .HasPrecision(18, 3);
        
        builder.Property(i => i.NotesAr)
            .HasMaxLength(1000);
        
        builder.Property(i => i.NotesEn)
            .HasMaxLength(1000);
        
        builder.Property(i => i.InternalNotes)
            .HasMaxLength(2000);
        
        // Relationships
        builder.HasOne(i => i.Branch)
            .WithMany(b => b.Invoices)
            .HasForeignKey(i => i.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(i => i.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// InvoiceLine entity configuration
/// </summary>
public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.ProductCode)
            .HasMaxLength(50);
        
        builder.Property(l => l.ProductNameAr)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(l => l.ProductNameEn)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(l => l.EgyptianBarcode)
            .HasMaxLength(20);
        
        builder.Property(l => l.UnitCode)
            .HasMaxLength(10);
        
        builder.Property(l => l.UnitPrice)
            .HasPrecision(18, 3);
        
        builder.Property(l => l.LineDiscountAmount)
            .HasPrecision(18, 3);
        
        builder.Property(l => l.TaxableAmount)
            .HasPrecision(18, 3);
        
        builder.Property(l => l.TaxAmount)
            .HasPrecision(18, 3);
        
        builder.Property(l => l.Subtotal)
            .HasPrecision(18, 3);
        
        builder.Property(l => l.TotalAmount)
            .HasPrecision(18, 3);
        
        builder.HasOne(l => l.Invoice)
            .WithMany(i => i.Lines)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Customer entity configuration
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.NameAr)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.NameEn)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(c => c.TaxRegistrationNumber)
            .IsUnique();
        
        builder.Property(c => c.TaxRegistrationNumber)
            .HasMaxLength(50);
        
        builder.Property(c => c.TaxCardNumber)
            .HasMaxLength(50);
        
        builder.Property(c => c.Email)
            .HasMaxLength(256);
        
        builder.Property(c => c.Phone)
            .HasMaxLength(20);
        
        builder.Property(c => c.Mobile)
            .HasMaxLength(20);
        
        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 3);
        
        builder.Property(c => c.CurrentBalance)
            .HasPrecision(18, 3);
    }
}

/// <summary>
/// User entity configuration
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(u => u.Username)
            .IsUnique();
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(u => u.FullNameAr)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.FullNameEn)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Phone)
            .HasMaxLength(20);
        
        builder.Property(u => u.Mobile)
            .HasMaxLength(20);
        
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);
        
        builder.Property(u => u.Language)
            .HasMaxLength(10);
    }
}
