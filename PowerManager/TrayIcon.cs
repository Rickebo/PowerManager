using Microsoft.Win32;

namespace PowerManager;

using System.Windows.Forms;

public class TrayIcon
{
    private Thread _uiThread;
    private readonly PowerSchemeCollection _schemeCollection;
    
    private ToolStripMenuItem ActivePlanItem { get; set; }
    private ToolStripMenuItem OpenConfigItem { get; set; }
    private ToolStripMenuItem ExitItem { get; set; }
    private ToolStripMenuItem AutoStartItem { get; set; }
    private ToolStripMenuItem CloseMenuItem { get; set; }
    private ToolStripMenuItem RestartItem { get; set; }
    
    
    public TrayIcon(PowerSchemeCollection schemeCollection)
    {
        _schemeCollection = schemeCollection;
        
        _uiThread = new Thread(Run);
        _uiThread.Start();
    }

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

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();
        
        menu.Text = "PowerManager";

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

        ActivePlanItem = new ToolStripMenuItem();
        AutoStartItem = new ToolStripMenuItem();
        CloseMenuItem = new ToolStripMenuItem()
        {
            Text = "Close this menu"
        };
        RestartItem = new ToolStripMenuItem()
        {
            Text = "Restart PowerManager"
        };
        RestartItem.Click += (s, a) => Restart();
        
        UpdateAll();

        AutoStartItem.Click += (s, a) => ToggleAutoStart();
        
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

    private void ToggleAutoStart()
    {
        var isAutoStart = IsAutoLaunch;

        if (isAutoStart)
            DisableAutoLaunch();
        else
            EnableAutoLaunch();

        UpdateAutoStart();
    }

    private void UpdateAutoStart()
    {
        AutoStartItem.Text = IsAutoLaunch 
            ? "Disable start with windows" 
            : "Start with windows";
    }
    
    private void UpdateActivePowerPlan()
    {
        ActivePlanItem.Text = $"Active power plan: {_schemeCollection.GetActive().Name}";
    }

    private void UpdateAll()
    {
        UpdateActivePowerPlan();
        UpdateAutoStart();
    }

    private void Restart()
    {
        Application.Restart();
        Environment.Exit(0);
    }
    
    private const string PowerManagerRegistryKey = "PowerManager";
    
    private static RegistryKey GetStartupKey() => 
        Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    public bool IsAutoLaunch => GetStartupKey().GetValue(PowerManagerRegistryKey) != null;

    public void EnableAutoLaunch() => GetStartupKey()
        .SetValue(PowerManagerRegistryKey, Application.ExecutablePath);
    
    
    public void DisableAutoLaunch() => GetStartupKey()
        .DeleteValue(PowerManagerRegistryKey, false);
}