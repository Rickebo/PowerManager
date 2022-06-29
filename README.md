# PowerManager

PowerManager is a simple .NET 6 application to automatically change Windows power plans depending on some circumstances. Currently, the application can change power plans if certain processes defined in the settings.json file are running or not. It can also be used to enforce a specific power plan.

## Prerequisites

- [.NET Runtime 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later
- Windows

## My use case

I decided to write this application so that I could use the high-performance power plan only when some applications are running.

## Usage

Download the latest release and save the files somewhere. Run ```PowerManager.exe```. A tray icon will appear. Right-click it, and select the option ```Open config file```.

Change the name of ```PerformancePlan``` to the name of the power plan you wish to have applied when the circumstances are met, and ```IdlePlan``` to the name of the plan to be applied otherwise. Note that the name of the power plans are likely different if your Windows installation is in some other language, but they should be the same as the names shown in Windows settings. The GUID of the power plans can also be used, and if one is specified, that will be used instead of the name. The default GUIDs should work even if your Windows installation is in some other language.

Also, change the ```Applications``` list to match your preferences. PowerManager will apply the plan specified under the setting ```PerformancePlan``` when it detects finds a process running with a name contained in the list. The process names to add to the ```Applications``` list are the same as shown under the details tab of Task Manager, without the trailing .exe. So, as an example, notepad is shown in the details tab of task manager as ```notepad.exe```, but ```notepad``` should be added to the list.

If you wish to use the program to force, for example, the ```High performance``` power plan to always be active, you can specify its GUID under the ```IdlePlan``` GUID as well, so that the two GUIDs match.

Finally, when you are comfortable with your settings, right-click the tray icon again and select ```Restart PowerManager``` to apply the settings.