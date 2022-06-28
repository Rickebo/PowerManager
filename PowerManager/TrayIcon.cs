using Microsoft.Win32;

namespace PowerManager;

using System.Windows.Forms;

public class TrayIcon
{
    /// <summary>
    /// The thread running the UI, or the NotifyIcon
    /// </summary>
    private readonly Thread _uiThread;

    /// <summary>
    /// The scheme collection used to find the currently active power plan
    /// </summary>
    private readonly PowerSchemeCollection _schemeCollection;
    
    /// <summary>
    /// The tool strip menu item showing the currently active power plan
    /// </summary>
    private ToolStripMenuItem ActivePlanItem { get; set; }

    /// <summary>
    /// The tool strip menu item representing opening the config file
    /// </summary>
    private ToolStripMenuItem OpenConfigItem { get; set; }

    /// <summary>
    /// The tool strip menu item representing exiting the application
    /// </summary>
    private ToolStripMenuItem ExitItem { get; set; }

    /// <summary>
    /// The tool strip menu item representing toggling whether or not the application
    /// starts with Windows
    /// </summary>
    private ToolStripMenuItem AutoStartItem { get; set; }

    /// <summary>
    /// The tool strip menu item representing closing the context menu
    /// </summary>
    private ToolStripMenuItem CloseMenuItem { get; set; }

    /// <summary>
    /// The tool strip menu item representing restarting the application
    /// </summary>
    private ToolStripMenuItem RestartItem { get; set; }
    
    
    /// <summary>
    /// Initializes the tray icon and runs it in another thread
    /// </summary>
    /// <param name="schemeCollection"></param>
    public TrayIcon(PowerSchemeCollection schemeCollection)
    {
        _schemeCollection = schemeCollection;
        
        _uiThread = new Thread(Run);
        _uiThread.Start();
    }

    /// <summary>
    /// Initializes and runs the notify icon used for the tray icon
    /// </summary>
    private void Run()
    {
        var icon = new NotifyIcon();
        icon.Icon = Resources.icon;
        icon.ContextMenuStrip = CreateContextMenu();
        icon.Visible = true;
        icon.Click += (s, a) => UpdateAll();
        icon.MouseClick += (s, a) => UpdateAll();

        Application.Run();
    }

    /// <summary>
    /// Create a context menu for the tray icon
    /// </summary>
    /// <returns>The newly created context menu</returns>
    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip()
        { 
            Text = "PowerManager"
        };

        OpenConfigItem = new ToolStripMenuItem()
        {
            Text = "Open config file"
        };

        OpenConfigItem.Click += (s, a) => Settings.OpenInEditor();

        ExitItem = new ToolStripMenuItem()
        {
            Text = "Exit"
        };

        ExitItem.Click += (s, a) => Environment.Exit(0);

        CloseMenuItem = new ToolStripMenuItem()
        {
            Text = "Close this menu"
        };

        RestartItem = new ToolStripMenuItem()
        {
            Text = "Restart PowerManager"
        };

        RestartItem.Click += (s, a) => Restart();
        
        ActivePlanItem = new ToolStripMenuItem();
        
        AutoStartItem = new ToolStripMenuItem();
        AutoStartItem.Click += (s, a) => ToggleAutoStart();

        UpdateAll();
        
        menu.Items.AddRange(new []
        {
            ActivePlanItem,
            CloseMenuItem,
            OpenConfigItem,
            AutoStartItem, 
            RestartItem,
            ExitItem
        });

        return menu;
    }

    /// <summary>
    /// Toggles whether the application starts with Windows or not
    /// </summary>
    private void ToggleAutoStart()
    {
        var isAutoStart = IsAutoLaunch;

        if (isAutoStart)
            DisableAutoLaunch();
        else
            EnableAutoLaunch();

        UpdateAutoStart();
    }

    /// <summary>
    /// Updates the auto start tool strip menu item
    /// </summary>
    private void UpdateAutoStart()
    {
        AutoStartItem.Text = IsAutoLaunch 
            ? "Disable start with windows" 
            : "Start with windows";
    }
    
    /// <summary>
    /// Updates the active power plan tool strip menu item
    /// </summary>
    private void UpdateActivePowerPlan()
    {
        ActivePlanItem.Text = $"Active power plan: {_schemeCollection.GetActive().Name}";
    }

    /// <summary>
    /// Updates all updateable tool strip menu items
    /// </summary>
    private void UpdateAll()
    {
        UpdateActivePowerPlan();
        UpdateAutoStart();
    }

    /// <summary>
    /// Restarts the application
    /// </summary>
    private void Restart()
    {
        Application.Restart();
        Environment.Exit(0);
    }
    
    /// <summary>
    /// The registry key to use for the application when setting it to start with windows
    /// </summary>
    private const string PowerManagerRegistryKey = "PowerManager";
    
    /// <summary>
    /// Finds the registry key containing programs to start with windows
    /// </summary>
    /// <returns></returns>
    private static RegistryKey GetStartupKey() => 
        Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    /// <summary>
    /// Whether or not the program is set to automatically launch with windows
    /// </summary>
    public bool IsAutoLaunch => GetStartupKey().GetValue(PowerManagerRegistryKey) != null;

    /// <summary>
    /// Enables launching the program automatically with Windows
    /// </summary>
    public void EnableAutoLaunch() => GetStartupKey()
        .SetValue(PowerManagerRegistryKey, Application.ExecutablePath);
    
    /// <summary>
    /// Disables launching the program automatically with Windows
    /// </summary>
    public void DisableAutoLaunch() => GetStartupKey()
        .DeleteValue(PowerManagerRegistryKey, false);
}