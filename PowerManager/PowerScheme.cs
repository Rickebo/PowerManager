namespace PowerManager;

using static Vanara.PInvoke.PowrProf;

public class PowerScheme
{
    private readonly PowerSchemeCollection _collection;
    public string Name { get; }
    public Guid Guid { get; }

    public PowerScheme(PowerSchemeCollection collection, Guid guid)
    {
        _collection = collection;
        
        Guid = guid;
        Name = PowerReadFriendlyName(guid);
    }

    public bool IsActive => _collection.ActiveGuid == Guid;

    public override bool Equals(object? obj) =>
        obj is PowerScheme other &&
        other.Guid == Guid;

    public override int GetHashCode() =>
        HashCode.Combine(typeof(PowerScheme).GetHashCode(), Guid.GetHashCode());
}