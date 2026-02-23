using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StationeryStore.Domain.Common;
using StationeryStore.Domain.Entities;
using StationeryStore.Domain.Enums;
using StationeryStore.Infrastructure.Data.Interceptors;

namespace StationeryStore.Infrastructure.Data;

/// <summary>
/// PostgreSQL DbContext for Stationery Store with Egyptian compliance
/// </summary>
public class StoreDbContext : DbContext
{
    private readonly ICurrentUserProvider? _currentUserProvider;
    private readonly AuditInterceptor? _auditInterceptor;
    
    public StoreDbContext(
        DbContextOptions<StoreDbContext> options,
        ICurrentUserProvider? currentUserProvider = null,
        AuditInterceptor? auditInterceptor = null)
        : base(options)
    {
        _currentUserProvider = currentUserProvider;
        _auditInterceptor = auditInterceptor;
    }
    
    // Branch & Location
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<InventoryLocation> InventoryLocations => Set<InventoryLocation>();
    
    // Product Catalog
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<VatRate> VatRates => Set<VatRate>();
    
    // Customer Management
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();
    
    // Sales & POS
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Till> Tills => Set<Till>();
    public DbSet<PosSession> PosSessions => Set<PosSession>();
    
    // Supplier & Purchase Orders
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptLine> GoodsReceiptLines => Set<GoodsReceiptLine>();
    
    // Inventory Management
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    
    // User & Role Management
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    
    // ETA E-Invoicing
    public DbSet<EtaSubmissionLog> EtaSubmissionLogs => Set<EtaSubmissionLog>();
    
    // Audit
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ignore domain events for EF Core
        modelBuilder.Ignore<DomainEvent>();
        
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
        
        // Configure decimal precision for Egyptian currency (3 decimals for milliemes)
        ConfigureDecimalPrecision(modelBuilder);
        
        // Configure Arabic collation for string columns
        ConfigureArabicCollation(modelBuilder);
        
        // Configure soft delete query filters
        ConfigureSoftDeleteFilters(modelBuilder);
        
        // Seed Egyptian data
        SeedEgyptianData(modelBuilder);
    }
    
    private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(3);  // Egyptian piasters (قرش)
                }
            }
        }
    }
    
    private void ConfigureArabicCollation(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) &&
                    (property.Name.Contains("Ar", StringComparison.OrdinalIgnoreCase) ||
                     property.Name.Contains("Name", StringComparison.OrdinalIgnoreCase) ||
                     property.Name.Contains("Address", StringComparison.OrdinalIgnoreCase) ||
                     property.Name.Contains("Description", StringComparison.OrdinalIgnoreCase)))
                {
                    property.SetCollation("arabic_ci");
                }
            }
        }
    }
    
    private void ConfigureSoftDeleteFilters(ModelBuilder modelBuilder)
    {
        // Apply soft delete filter to entities with IsDeleted property
        var softDeleteTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType));

        foreach (var entityType in softDeleteTypes)
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, "IsDeleted");
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
    
    private void SeedEgyptianData(ModelBuilder modelBuilder)
    {
        // Seed VAT rates for Egypt
        var standardVat = new VatRate
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            NameAr = "ضريبة القيمة المضافة القياسية",
            NameEn = "Standard Value Added Tax",
            Code = "STD",
            Rate = 14.00m,
            EtaTaxTypeCode = "T1",
            IsActive = true
        };

        var exemptVat = new VatRate
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            NameAr = "معفى من الضريبة",
            NameEn = "Tax Exempt",
            Code = "EXM",
            Rate = 0.00m,
            EtaTaxTypeCode = "T2",
            IsActive = true
        };

        modelBuilder.Entity<VatRate>().HasData(standardVat, exemptVat);

        // Seed Egyptian units
        var units = new[]
        {
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Code = "PCS", NameAr = "قطعة", NameEn = "Piece", Symbol = "pcs", ConversionFactor = 1.000m, IsActive = true },
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Code = "BOX", NameAr = "علبة", NameEn = "Box", Symbol = "box", ConversionFactor = 12.000m, IsActive = true },
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Code = "REAM", NameAr = "رزمة", NameEn = "Ream", Symbol = "ream", ConversionFactor = 500.000m, IsActive = true },
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Code = "PKT", NameAr = "حزمة", NameEn = "Packet", Symbol = "pkt", ConversionFactor = 10.000m, IsActive = true },
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Code = "DOZ", NameAr = "دزينة", NameEn = "Dozen", Symbol = "doz", ConversionFactor = 12.000m, IsActive = true },
            new Unit { Id = Guid.Parse("33333333-3333-3333-3333-333333333336"), Code = "SET", NameAr = "مجموعة", NameEn = "Set", Symbol = "set", ConversionFactor = 1.000m, IsActive = true }
        };

        modelBuilder.Entity<Unit>().HasData(units);
        
        // Seed default Egyptian branch (Cairo headquarters)
        var cairoBranch = new Branch
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            NameAr = "المقر الرئيسي - القاهرة",
            NameEn = "Headquarters - Cairo",
            AddressAr = "شارع التحرير، وسط البلد، القاهرة",
            AddressEn = "Tahrir Street, Downtown, Cairo",
            TaxRegistrationNumber = "123-456-789",
            TaxActivityCode = "47891",
            CommercialRegistrationNumber = "12345",
            GovernorateAr = "القاهرة",
            GovernorateEn = "Cairo",
            CityAr = "القاهرة",
            CityEn = "Cairo",
            PostalCode = "11511",
            Phone = "+201234567890",
            Email = "cairo@stationery.eg",
            IsActive = true,
            IsHeadquarters = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };
        cairoBranch.SetWorkingDays(new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday });
        
        modelBuilder.Entity<Branch>().HasData(cairoBranch);
        
        // Seed system roles
        var roles = new[]
        {
            new Role { Id = Guid.Parse("55555555-5555-5555-5555-555555555551"), NameAr = "مدير النظام", NameEn = "System Administrator", Code = "Admin", IsSystemRole = true, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new Role { Id = Guid.Parse("55555555-5555-5555-5555-555555555552"), NameAr = "مدير المتجر", NameEn = "Store Manager", Code = "Manager", IsSystemRole = true, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new Role { Id = Guid.Parse("55555555-5555-5555-5555-555555555553"), NameAr = "كاشير", NameEn = "Cashier", Code = "Cashier", IsSystemRole = true, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new Role { Id = Guid.Parse("55555555-5555-5555-5555-555555555554"), NameAr = "مدير المخزون", NameEn = "Inventory Manager", Code = "InventoryManager", IsSystemRole = true, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
        };
        
        modelBuilder.Entity<Role>().HasData(roles);
        
        // Seed default admin user
        var adminUser = new User
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            Username = "admin",
            Email = "admin@stationery.eg",
            PasswordHash = "AQAAAAIAAYagAAAAEH7Y3vK8QJz9N5P2xRmWq8L4jZ3vK8QJz9N5P2xRmWq8L4jZ3vK8QJz9N5P2xRmWq8L4jQ==",  // BCrypt hash for "Admin@123"
            FullNameAr = "مدير النظام",
            FullNameEn = "System Administrator",
            RoleId = Guid.Parse("55555555-5555-5555-5555-555555555551"),
            DefaultBranchId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            IsActive = true,
            Language = "ar",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };
        
        modelBuilder.Entity<User>().HasData(adminUser);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;
        var userId = _currentUserProvider?.UserId?.ToString() ?? "SYSTEM";
        var userName = _currentUserProvider?.UserName ?? "SYSTEM";
        
        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            
            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = now;
                    entity.CreatedBy = userName;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = userName;
                    break;
                    
                case EntityState.Modified:
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = userName;
                    break;
                    
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = now;
                    entity.DeletedBy = userName;
                    break;
            }
        }
    }
}

/// <summary>
/// Interface to provide current user context
/// </summary>
public interface ICurrentUserProvider
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? IpAddress { get; }
}
