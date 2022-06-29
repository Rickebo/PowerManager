using System.Diagnostics;
using Newtonsoft.Json;

namespace PowerManager;

public class Settings
{
    public static string BasePath { get; }= Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "PowerManager");
    
    public static string SettingsFile { get; } = Path.Combine(BasePath, "settings.json");
    
    /// <summary>
    /// The list of applications to apply the performance plan for
    /// </summary>
    public List<string>? Applications { get; set; }

    /// <summary>
    /// The plan to apply when an application from <cref name="Applications">applications</cref> is running.
    /// </summary>
    public PowerSchemeSettings? PerformancePlan { get; set; }

    /// <summary>
    /// The plan to apply when no application from <cref name="Applications">applications</cref> is running.
    /// </summary>
    public PowerSchemeSettings? IdlePlan { get; set; }

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
            MessageBox.Show(Resources.Settings_OpenInEditor_Error + 
                            SettingsFile, Resources.Settings_OpenInEditor_Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    /// <summary>
    /// Template settings to use as default
    /// </summary>
    private static Settings TemplateSettings { get; } =
        new()
        {
            Applications = new (Resources.Settings_Default_Application.Split(',')),
            PerformancePlan = new ()
            {
                Guid = Resources.Settings_Default_PerformanceGuid,
                Name = Resources.Settings_Default_PerformanceName
            },
            IdlePlan = new ()
            {
                Guid = Resources.Settings_Default_IdleGuid,
                Name = Resources.Settings_Default_IdleName   
            }
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
        
        Console.WriteLine(Resources.Settings_Setup_NotFound);
        settings = TemplateSettings;
        File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settings, Formatting.Indented));

        return settings;
    }
}