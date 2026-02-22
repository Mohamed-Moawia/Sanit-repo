namespace StationeryStore.Domain.Common;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
    
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}
