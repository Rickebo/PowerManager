using Vanara.PInvoke;

namespace PowerManager;

using static Vanara.PInvoke.PowrProf;

public class PowerSchemeCollection : Dictionary<Guid, PowerScheme>
{
    public Guid ActiveGuid { get; private set; } 
    
    public PowerSchemeCollection()
    {
        Update();

        foreach (var scheme in PowerEnumerate<Guid>(null, null))
            this[scheme] = new PowerScheme(this, scheme);
    }
    
    public void Update()
    {
        PowerGetActiveScheme(out var active);
        ActiveGuid = active;
    }

    public void SetActive(PowerScheme scheme) =>
        SetActive(scheme.Guid);
    
    public void SetActive(Guid guid)
    {
        PowerSetActiveScheme(HKEY.NULL, guid);
    }

    public PowerScheme GetActive() => 
        this[ActiveGuid];
    
    public PowerScheme GetScheme(string name) => 
        Values.First(scheme => scheme.Name == name);
}