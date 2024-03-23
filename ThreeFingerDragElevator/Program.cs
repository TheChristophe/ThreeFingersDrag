﻿using System.ComponentModel;
using System.Diagnostics;

var processInfo = new ProcessStartInfo
{
    UseShellExecute = true,
    FileName = "ThreeFingerDragOnWindows.exe",
    Arguments = "FromElevator"
};
try
{
    Process.Start(processInfo);
}
catch (Win32Exception ex)
{
    Debug.WriteLine(ex);
}