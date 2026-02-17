// Domain/Entities/Showroom.cs
public class Showroom
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public GeoLocation Coordinates { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public BusinessHours OperatingHours { get; private set; }
    public List<DisplayArea> DisplayAreas { get; private set; }
}
