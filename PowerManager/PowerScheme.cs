namespace PowerManager;

using static Vanara.PInvoke.PowrProf;

public class PowerScheme
{
    /// <summary>
    /// A provider of the current active power plan to use to determine if the current plan is active
    /// </summary>
    private readonly IActivePowerPlanProvider _activePlanProvider;

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
    /// <param name="activePlanProvider">The provider of active power plan to use for determining if the plan is active
    /// or not</param>
    /// <param name="guid">The guid of the power plan to create</param>
    public PowerScheme(IActivePowerPlanProvider activePlanProvider, Guid guid)
    {
        _activePlanProvider = activePlanProvider;
        
        Guid = guid;
        Name = PowerReadFriendlyName(guid);
    }

    /// <summary>
    /// Whether or not the power plan is the currently active power plan
    /// </summary>
    public bool IsActive => _activePlanProvider.ActivePlanGuid == Guid;
    
    public override bool Equals(object? obj) =>
        obj is PowerScheme other &&
        other.Guid == Guid;

    public override int GetHashCode() =>
        HashCode.Combine(typeof(PowerScheme).GetHashCode(), Guid.GetHashCode());
}