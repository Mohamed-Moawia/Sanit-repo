using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationeryStore.Application.Common;
using StationeryStore.Application.DTOs;
using StationeryStore.Application.Interfaces;
using StationeryStore.Domain.Entities;
using StationeryStore.Infrastructure.Data;

namespace StationeryStore.API.Controllers;

/// <summary>
/// Products management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly StoreDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(StoreDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all products with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<ProductDto>>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? barcode = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? branchId = null,
        [FromQuery] bool? lowStock = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.VatRate)
                .Include(p => p.Branch)
                .AsQueryable();
            
            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.NameAr.Contains(search) ||
                    p.NameEn.Contains(search) ||
                    p.Sku.Contains(search) ||
                    (p.EgyptianBarcode != null && p.EgyptianBarcode.Contains(search)));
            }
            
            if (!string.IsNullOrEmpty(barcode))
            {
                query = query.Where(p => p.EgyptianBarcode == barcode || p.Sku == barcode);
            }
            
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            
            if (branchId.HasValue)
            {
                query = query.Where(p => p.BranchId == branchId.Value);
            }
            
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }
            
            if (lowStock == true)
            {
                query = query.Where(p => p.MinimumStockLevel > 0 &&
                                         (p.StockQuantity ?? 0) <= p.ReorderPoint);
            }
            
            var totalItems = await query.CountAsync();
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Sku,
                    p.NameAr,
                    p.NameEn,
                    p.EgyptianBarcode,
                    p.CategoryId,
                    p.Category.NameAr,
                    p.UnitId,
                    p.Unit.Code,
                    p.CostPrice,
                    p.SalePrice,
                    p.VatRateId,
                    p.VatRate.Rate,
                    p.StockQuantity,
                    p.MinimumStockLevel,
                    p.ReorderPoint,
                    p.IsActive,
                    p.EtaItemCode,
                    p.EtaUnitCode,
                    p.CreatedAt))
                .ToListAsync();
            
            var pagedResponse = PagedResponse<ProductDto>.Create(products, totalItems, page, pageSize);
            return Ok(ApiResponse<PagedResponse<ProductDto>>.SuccessResponse(pagedResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, ApiResponse<PagedResponse<ProductDto>>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProduct(Guid id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.VatRate)
                .Include(p => p.Branch)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.FailureResponse("Product not found"));
            
            var response = new ProductDetailDto(
                product.Id,
                product.Sku,
                product.NameAr,
                product.NameEn,
                product.DescriptionAr,
                product.DescriptionEn,
                product.EgyptianBarcode,
                product.CategoryId,
                product.Category?.NameAr,
                product.Category?.NameEn,
                product.UnitId,
                product.Unit?.Code,
                product.Unit?.NameAr,
                product.CostPrice,
                product.SalePrice,
                product.WholesalePrice,
                product.VatRateId,
                product.VatRate?.Rate ?? 14.0m,
                product.VatRate?.EtaTaxTypeCode,
                product.StockQuantity,
                product.MinimumStockLevel,
                product.MaximumStockLevel,
                product.ReorderPoint,
                product.ReorderQuantity,
                product.IsActive,
                product.EtaItemCode,
                product.EtaUnitCode,
                product.BranchId,
                product.Branch?.NameAr,
                new List<string>(),
                new Dictionary<string, string>(),
                product.CreatedAt,
                product.UpdatedAt);
            
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDetailDto>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Get product by barcode
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductByBarcode(string barcode)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.VatRate)
                .FirstOrDefaultAsync(p =>
                    (p.EgyptianBarcode == barcode || p.Sku == barcode) &&
                    !p.IsDeleted && p.IsActive);
            
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.FailureResponse("Product not found"));
            
            var response = new ProductDetailDto(
                product.Id,
                product.Sku,
                product.NameAr,
                product.NameEn,
                product.DescriptionAr,
                product.DescriptionEn,
                product.EgyptianBarcode,
                product.CategoryId,
                product.Category?.NameAr,
                null,
                product.UnitId,
                product.Unit?.Code,
                null,
                product.CostPrice,
                product.SalePrice,
                product.WholesalePrice,
                product.VatRateId,
                product.VatRate?.Rate ?? 14.0m,
                product.VatRate?.EtaTaxTypeCode,
                product.StockQuantity,
                product.MinimumStockLevel,
                product.MaximumStockLevel,
                product.ReorderPoint,
                product.ReorderQuantity,
                product.IsActive,
                product.EtaItemCode,
                product.EtaUnitCode,
                product.BranchId,
                product.Branch?.NameAr,
                new List<string>(),
                new Dictionary<string, string>(),
                product.CreatedAt,
                product.UpdatedAt);
            
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by barcode {Barcode}", barcode);
            return StatusCode(500, ApiResponse<ProductDetailDto>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            // Validate category
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest(ApiResponse<ProductDetailDto>.FailureResponse("Category not found"));
            
            // Validate unit
            var unit = await _context.Units.FindAsync(request.UnitId);
            if (unit == null)
                return BadRequest(ApiResponse<ProductDetailDto>.FailureResponse("Unit not found"));
            
            // Validate VAT rate
            var vatRate = await _context.VatRates.FindAsync(request.VatRateId);
            if (vatRate == null)
                return BadRequest(ApiResponse<ProductDetailDto>.FailureResponse("VAT rate not found"));
            
            // Check for duplicate barcode
            if (!string.IsNullOrEmpty(request.EgyptianBarcode))
            {
                if (await _context.Products.AnyAsync(p => p.EgyptianBarcode == request.EgyptianBarcode))
                    return BadRequest(ApiResponse<ProductDetailDto>.FailureResponse("Barcode already exists"));
            }
            
            var product = new Product
            {
                Sku = request.Sku,
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                DescriptionAr = request.DescriptionAr,
                DescriptionEn = request.DescriptionEn,
                EgyptianBarcode = request.EgyptianBarcode,
                CategoryId = request.CategoryId,
                UnitId = request.UnitId,
                VatRateId = request.VatRateId,
                BranchId = request.BranchId,
                CostPrice = request.CostPrice,
                SalePrice = request.SalePrice,
                MinimumStockLevel = request.MinimumStockLevel,
                MaximumStockLevel = request.MaximumStockLevel,
                ReorderPoint = request.ReorderPoint,
                ReorderQuantity = request.ReorderQuantity,
                IsActive = request.IsActive,
                EtaItemCode = request.EtaItemCode,
                EtaUnitCode = request.EtaUnitCode
            };
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Product {ProductName} created successfully", product.NameAr);
            
            var response = new ProductDetailDto(
                product.Id,
                product.Sku,
                product.NameAr,
                product.NameEn,
                product.DescriptionAr,
                product.DescriptionEn,
                product.EgyptianBarcode,
                product.CategoryId,
                category.NameAr,
                category.NameEn,
                product.UnitId,
                unit.Code,
                unit.NameAr,
                product.CostPrice,
                product.SalePrice,
                product.WholesalePrice,
                product.VatRateId,
                vatRate.Rate,
                vatRate.EtaTaxTypeCode,
                product.StockQuantity,
                product.MinimumStockLevel,
                product.MaximumStockLevel,
                product.ReorderPoint,
                product.ReorderQuantity,
                product.IsActive,
                product.EtaItemCode,
                product.EtaUnitCode,
                product.BranchId,
                null,
                new List<string>(),
                new Dictionary<string, string>(),
                product.CreatedAt,
                product.UpdatedAt);
            
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id },
                ApiResponse<ProductDetailDto>.SuccessResponse(response, "Product created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, ApiResponse<ProductDetailDto>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Update product stock
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
                return NotFound(ApiResponse<bool>.FailureResponse("Product not found"));
            
            product.StockQuantity = request.Quantity;
            await _context.SaveChangesAsync();
            
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Stock updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", id);
            return StatusCode(500, ApiResponse<bool>.FailureResponse("An error occurred"));
        }
    }
}
