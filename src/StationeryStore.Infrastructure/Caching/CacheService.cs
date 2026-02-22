using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;

namespace StationeryStore.Infrastructure.Caching;

/// <summary>
/// Redis cache service implementation
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly CacheSettings _settings;
    
    public CacheService(IDistributedCache cache, IOptions<CacheSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(cachedValue))
            return default;
        
        return System.Text.Json.JsonSerializer.Deserialize<T>(cachedValue);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_settings.AbsoluteExpirationInMinutes),
            SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpirationInMinutes)
        };
        
        var serializedValue = System.Text.Json.JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }
    
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _cache.RemoveAsync(key, cancellationToken);
    
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: Redis pattern deletion requires SCAN command which is not directly available
        // This is a simplified implementation
        // For production, consider using Redis keyspace notifications or a separate index
        await Task.CompletedTask;
    }
    
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetAsync(key, cancellationToken);
        return value != null;
    }
}
