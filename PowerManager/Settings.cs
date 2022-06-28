using System.Diagnostics;
using Newtonsoft.Json;

namespace PowerManager;

public class Settings
{
    private static readonly string BasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "PowerManager");
    
    private static readonly string SettingsFile = Path.Combine(BasePath, "settings.json");
    
    public List<string> Applications { get; set; }
    public string PerformancePlan { get; set; }
    public string IdlePlan { get; set; }

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