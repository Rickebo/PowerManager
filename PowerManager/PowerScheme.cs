namespace PowerManager;

using static Vanara.PInvoke.PowrProf;

public class PowerScheme
{
    /// <summary>
    /// A power plan collection to use to determine if current plan is active or not
    /// </summary>
    private readonly PowerSchemeCollection _collection;

    /// <summary>
    /// The name of the power plan
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The guid of the power plan
    /// </summary>
    public Guid Guid { get; }

    /// <summary>
    /// Creates a power plan using a specified power plan collection and guid
    /// </summary>
    /// <param name="collection">The power plan collection to use</param>
    /// <param name="guid">The guid of the power plan to create</param>
    public PowerScheme(PowerSchemeCollection collection, Guid guid)
    {
        _collection = collection;
        
        Guid = guid;
        Name = PowerReadFriendlyName(guid);
    }

    /// <summary>
    /// Whether or not the power plan is the currently active power plan
    /// </summary>
    public bool IsActive => _collection.ActiveGuid == Guid;
    
    public override bool Equals(object? obj) =>
        obj is PowerScheme other &&
        other.Guid == Guid;

    public override int GetHashCode() =>
        HashCode.Combine(typeof(PowerScheme).GetHashCode(), Guid.GetHashCode());
}