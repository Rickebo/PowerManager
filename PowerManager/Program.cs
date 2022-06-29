using System.Diagnostics;
using PowerManager;

// Read settings from file, or use default settings and write template
// settings to file
var settings = Settings.Setup();

// Store applications in a case-insensitive hashset for faster lookups
var applicationsSet = new HashSet<string>(
    settings.Applications ?? new List<string>(), 
    StringComparer.OrdinalIgnoreCase);

if (settings.PerformancePlan == null || settings.IdlePlan == null)
    throw new Exception("Performance and/or idle plan not set");

// Find all power plans that exist (as of starting the application)
var plans = new PowerSchemeCollection();

// Find the power plan used as "performance plan"
var performanceScheme = plans.GetScheme(settings.PerformancePlan);

// Find the power plan to use for idling
var idleScheme = plans.GetScheme(settings.IdlePlan);

if (performanceScheme == null)
{
    MessageBox.Show(
        text: string.Format(
            Resources.Program_Error_PerformanceSchemeNull,
            settings.PerformancePlan, 
            Settings.SettingsFile
        ),
        caption: Resources.Program_Error_PerformanceSchemeNull_Title,
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation);

    return;
}

if (idleScheme == null)
{
    MessageBox.Show(
        text: string.Format(
            Resources.Program_Error_IdleSchemeNull, 
            settings.IdlePlan, 
            Settings.SettingsFile
        ),
        caption: Resources.Program_Error_IdleSchemeNull_Title,
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation);

    return;
}

// Create a tray icon with context menu
var trayIcon = new TrayIcon(plans);
trayIcon.Show();

// Create a mutex to use to ensure updates don't run concurrently
var mutex = new Mutex();

// Create a timer using the update interval from settings
var timer = new System.Timers.Timer()
{
        Interval = settings.UpdateInterval,
        Enabled = true
};

timer.Elapsed += (_, _) => Update();
timer.Start();

void Update()
{
    // Wait for mutex to ensure that update is not running concurrently
    // If mutex is not instantly available, skip updating to prevent a growing
    // queue of updates from forming
    if (!mutex.WaitOne(0))
        return;

    try
    {
        var processes = Process.GetProcesses();
        var foundPrioritizedProcess = processes
                .Any(proc => applicationsSet.Contains(proc.ProcessName));

        var preferredPlan = foundPrioritizedProcess
                ? performanceScheme
                : idleScheme;

        plans.Update();
        // New plan is the same as the old plan, no need to set it as active 
        // again or to update the collection
        if (plans.ActivePlanGuid == preferredPlan.Guid)
            return;

        plans.SetActive(preferredPlan);
        plans.Update();
    }
    finally
    {
        mutex.ReleaseMutex();
    }
}

// Sleep indefinitely in so program doesn't exit due to main thread exit
Thread.Sleep(-1);