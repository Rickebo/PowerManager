using System.Diagnostics;
using Newtonsoft.Json;
using PowerManager;

var settings = Settings.Setup();

var applicationsSet = new HashSet<string>(
    settings.Applications, 
    StringComparer.OrdinalIgnoreCase);



var plans = new PowerSchemeCollection();
var performanceScheme = plans.GetScheme(settings.PerformancePlan);
var idleScheme = plans.GetScheme(settings.IdlePlan);

var trayIcon = new TrayIcon(plans);

var mutex = new Mutex();

var timer = new System.Timers.Timer()
{
        Interval = settings.UpdateInterval,
        Enabled = true
};

timer.Elapsed += (s, a) =>
{
    if (!mutex.WaitOne(0))
        return;

    try
    {
        var processes = Process.GetProcesses();
        var foundPrioritizedProcess = processes
            .Any(proc => applicationsSet.Contains(proc.ProcessName));

        plans.SetActive(foundPrioritizedProcess 
            ? performanceScheme 
            : idleScheme);

        plans.Update();
    }
    finally
    {
        mutex.ReleaseMutex();
    }
};

timer.Start();

Thread.Sleep(-1);