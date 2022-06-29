using Vanara.PInvoke;

namespace PowerManager;

using static PowrProf;

public class PowerSchemeCollection : Dictionary<Guid, PowerScheme>, IActivePowerPlanProvider
{
    /// <summary>
    /// The Guid of the currently active power plan
    /// </summary>
    public Guid ActivePlanGuid { get; private set; } 
    
    /// <summary>
    /// Creates and initializes a power scheme collection
    /// </summary>
    public PowerSchemeCollection()
    {
        Update();

        foreach (var scheme in PowerEnumerate<Guid>(null, null))
            this[scheme] = new PowerScheme(this, scheme);
    }
    
    /// <summary>
    /// Updates the active power plan
    /// </summary>
    public void Update()
    {
        PowerGetActiveScheme(out var active);
        ActivePlanGuid = active;
    }

    /// <summary>
    /// Sets the active power plan to a specified power plan
    /// </summary>
    /// <param name="scheme">The power plan to use</param>
    public void SetActive(PowerScheme scheme) =>
        SetActive(scheme.Guid);
    
    /// <summary>
    /// Sets the active power plan to a specified power plan using it's guid
    /// </summary>
    /// <param name="guid">The guid of the power plan to apply</param>
    public void SetActive(Guid guid) => 
        PowerSetActiveScheme(HKEY.NULL, guid);

    /// <summary>
    /// Gets the active power plan
    /// </summary>
    /// <returns></returns>
    public PowerScheme GetActive() => 
        this[ActivePlanGuid];
    
    /// <summary>
    /// Gets a power plan based on it's name. Search is case insensitive.
    /// </summary>
    /// <param name="name">The name of the power plan to get</param>
    /// <returns></returns>
    public PowerScheme? GetScheme(string name) => 
        Values.FirstOrDefault(scheme => scheme.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

    /// <summary>
    /// Gets a power plan based on it's name. Search is case insensitive.
    /// </summary>
    /// <param name="schemeSettings">The settings of the power plan to find</param>
    /// <returns>The power scheme, if found, otherwise, null</returns>
    public PowerScheme? GetScheme(PowerSchemeSettings schemeSettings) =>
        schemeSettings.Guid != null &&
        schemeSettings.ParseGuid(out var parsedGuid) &&
        TryGetValue(parsedGuid, out var foundScheme)
            ? foundScheme
            : schemeSettings.Name != null
                ? GetScheme(schemeSettings.Name)
                : null;
}