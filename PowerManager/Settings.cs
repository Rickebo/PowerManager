using System.Diagnostics;
using Newtonsoft.Json;

namespace PowerManager;

public class Settings
{
    private static readonly string BasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "PowerManager");
    
    private static readonly string SettingsFile = Path.Combine(BasePath, "settings.json");
    
    /// <summary>
    /// The list of applications to apply the performance plan for
    /// </summary>
    public List<string> Applications { get; set; }

    /// <summary>
    /// The plan to apply when an application from <param name="Applications">applications</param> is running.
    /// </summary>
    public string PerformancePlan { get; set; }

    /// <summary>
    /// The plan to apply when no application from <param name="Applications">applications</param> is running.
    /// </summary>
    public string IdlePlan { get; set; }

    /// <summary>
    /// How often the application should check running processes and update, in milliseconds
    /// </summary>
    public int UpdateInterval { get; set; } = 2000;

    /// <summary>
    /// Opens the config file in the default .json editor
    /// </summary>
    public static void OpenInEditor()
    {
        try
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "explorer",
                Arguments = "\"" + SettingsFile + "\""
            };
            
            Process.Start(psi);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageBox.Show("Failed to open config file in editor. Try opening the following file manually: \n" + 
                            SettingsFile, "Failed to open editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    /// <summary>
    /// Template settings to use as default
    /// </summary>
    public static Settings TemplateSettings { get; } =
        new()
        {
            Applications = new ()
            {
                "csgo"
            },
            PerformancePlan = "High performance",
            IdlePlan = "Balanced"
        };

    /// <summary>
    /// Sets up settings.
    /// Reads settings from file if file exists, otherwise writes default settings to file and returns them
    /// </summary>
    /// <returns>The settings from file, or sometimes default settings</returns>
    public static Settings Setup()
    {
        if (!Directory.Exists(BasePath))
            Directory.CreateDirectory(BasePath);

        var settings = File.Exists(SettingsFile)
            ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile))
            : null;

        if (settings != null) 
            return settings;
        
        Console.WriteLine("Found no valid settings, wrote template ones to file.");
        settings = TemplateSettings;
        File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settings, Formatting.Indented));

        return settings;
    }
}