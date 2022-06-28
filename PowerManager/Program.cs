using System.Diagnostics;
using Newtonsoft.Json;
using PowerManager;

// Read settings from file, or use default settings and write template
// settings to file
var settings = Settings.Setup();

// Store applications in a case-insensitive hashset for faster lookups
var applicationsSet = new HashSet<string>(
    settings.Applications, 
    StringComparer.OrdinalIgnoreCase);

// Find all power plans that exist (as of starting the application)
var plans = new PowerSchemeCollection();

// Find the power plan used as "performance plan"
var performanceScheme = plans.GetScheme(settings.PerformancePlan);

// Find the power plan to use for idling
var idleScheme = plans.GetScheme(settings.IdlePlan);

// Create a tray icon with context menu
var trayIcon = new TrayIcon(plans);

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

        // New plan is the same as the old plan, no need to set it as active 
        // again or to update the collection
        if (plans.ActiveGuid == preferredPlan.Guid)
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