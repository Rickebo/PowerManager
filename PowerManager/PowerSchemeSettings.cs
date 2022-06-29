namespace PowerManager;

public class PowerSchemeSettings
{
    /// <summary>
    /// The name of the power plan
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The guid of the power plan, in string format
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// Tries to parse the guid from <cref name="Guid"/> into <cref name="parsedGuid"/>.
    /// </summary>
    /// <param name="parsedGuid">Receives the resulting parsed guid, if parsing was successful</param>
    /// <returns>Whether or not the guid could be successfully parsed</returns>
    public bool ParseGuid(out Guid parsedGuid) => 
        System.Guid.TryParse(Guid, out parsedGuid);
}