using MediatR;

namespace StationeryStore.Application.Common;

/// <summary>
/// Standard response wrapper for API responses
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? MessageAr { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static ApiResponse<T> SuccessResponse(T data, string? message = null, string? messageAr = null)
        => new() { Success = true, Data = data, Message = message ?? "Operation successful", MessageAr = messageAr };
    
    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null, string? messageAr = null)
        => new() { Success = false, Message = message, MessageAr = messageAr, Errors = errors ?? new List<string>() };
}

/// <summary>
/// Paginated response for list endpoints
/// </summary>
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    
    public PagedResponse() { }
    
    public PagedResponse(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
    
    public static PagedResponse<T> Create(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
        => new(items, totalItems, currentPage, pageSize);
}

/// <summary>
/// Base request model with common properties
/// </summary>
public abstract class BaseRequest
{
    public string? RequestId { get; set; } = Guid.NewGuid().ToString();
}

/// <summary>
/// Base command model
/// </summary>
public abstract class BaseCommand : IRequest<ApiResponse<Guid>>
{
    public string? RequestId { get; set; } = Guid.NewGuid().ToString();
}

/// <summary>
/// Base query model
/// </summary>
public abstract class BaseQuery<T> : IRequest<ApiResponse<T>>
{
    public string? RequestId { get; set; } = Guid.NewGuid().ToString();
}
