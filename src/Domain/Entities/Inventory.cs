// Domain/Entities/Inventory.cs
public class Inventory
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid LocationId { get; private set; }
    public LocationType LocationType { get; private set; }
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public int MaximumStockLevel { get; private set; }
    public string BinLocation { get; private set; }
    
    public int AvailableQuantity => Quantity - ReservedQuantity;
    
    public void UpdateStock(int quantity, StockUpdateReason reason)
    {
        // Business logic for stock updates
    }
}
