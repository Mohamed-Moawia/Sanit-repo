using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StationeryStore.Domain.Common;
using StationeryStore.Domain.Entities;

namespace StationeryStore.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor for auditing entity changes
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserProvider? _currentUserProvider;
    
    public AuditInterceptor(ICurrentUserProvider? currentUserProvider = null)
    {
        _currentUserProvider = currentUserProvider;
    }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        
        var userId = _currentUserProvider?.UserId?.ToString() ?? "SYSTEM";
        var userName = _currentUserProvider?.UserName ?? "SYSTEM";
        var ipAddress = _currentUserProvider?.IpAddress;
        
        var auditEntries = new List<AuditTrail>();
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTrail || entry.Entity is BaseEntity == false)
                continue;
            
            var auditEntry = new AuditTrail
            {
                Id = Guid.NewGuid(),
                TableName = entry.Entity.GetType().Name,
                RecordId = GetPrimaryKeyValue(entry),
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName
            };
            
            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.Action = AuditAction.Create;
                    auditEntry.NewValues = SerializeCurrentValues(entry);
                    break;
                    
                case EntityState.Modified:
                    auditEntry.Action = AuditAction.Update;
                    auditEntry.AffectedColumns = SerializeAffectedColumns(entry);
                    auditEntry.OldValues = SerializeOriginalValues(entry);
                    auditEntry.NewValues = SerializeCurrentValues(entry);
                    break;
                    
                case EntityState.Deleted:
                    auditEntry.Action = AuditAction.SoftDelete;
                    auditEntry.OldValues = SerializeOriginalValues(entry);
                    break;
            }
            
            auditEntries.Add(auditEntry);
        }
        
        if (auditEntries.Any())
        {
            await context.AddRangeAsync(auditEntries, cancellationToken);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private static Guid GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Entity.GetType().GetProperty("Id");
        return key != null ? (Guid)key.GetValue(entry.Entity)! : Guid.Empty;
    }
    
    private static string SerializeCurrentValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            values[property.Metadata.Name] = property.CurrentValue;
        }
        return System.Text.Json.JsonSerializer.Serialize(values);
    }
    
    private static string SerializeOriginalValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            values[property.Metadata.Name] = property.OriginalValue;
        }
        return System.Text.Json.JsonSerializer.Serialize(values);
    }
    
    private static string SerializeAffectedColumns(EntityEntry entry)
    {
        var affectedColumns = entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name)
            .ToList();
        return System.Text.Json.JsonSerializer.Serialize(affectedColumns);
    }
}
