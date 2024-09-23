/// <summary>Launch the shortcut's target PowerShell script with the markdown.</summary>
/// <version>0.0.1.0</version>

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualBasic;

namespace cvmd2html
{
  static class Program
  {
    static void Main()
    {
      RequestAdminPrivileges();

      /** The application execution. */
      if (!String.IsNullOrEmpty(Parameters.Markdown))
      {
        const string CMD_EXE = @"C:\Windows\System32\cmd.exe";
        const string CMD_LINE_FORMAT = @"/d /c """"{0}"" 2> ""{1}""""";
        Package.IconLink.Create(Parameters.Markdown);
        if (WaitForExit(Process.Start(
          new ProcessStartInfo
          {
            FileName = CMD_EXE,
            Arguments = String.Format(CMD_LINE_FORMAT, Package.IconLink.Path, ErrorLog.Path),
            WindowStyle = ProcessWindowStyle.Hidden
          }
        ).Id) != 0)
        {
          ErrorLog.Read();
          ErrorLog.Delete();
        }
        Package.IconLink.Delete();
        Quit(0);
      }

      /** Configuration and settings. */
      if (Parameters.Set ^ Parameters.Unset)
      {
        if (Parameters.Set)
        {
          Setup.Set();
          if ((bool)Parameters.NoIcon)
          {
            Setup.RemoveIcon();
          }
          else
          {
            Setup.AddIcon();
          }
        }
        else if (Parameters.Unset)
        {
          Setup.Unset();
        }
        Quit(0);
      }

      Quit(1);
    }

    /// <summary>The path to the application.</summary>
    internal static readonly string Path = Assembly.GetExecutingAssembly().Location;

    /// <summary>Wait for the process exit.</summary>
    /// <param name="processId">The process identifier.</param>
    /// <returns>The exit status of the process.</returns>
    static int WaitForExit(int processId)
    {
      // The process termination event query. Win32_ProcessStopTrace requires admin rights to be used.
      var wmiQuery = "SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName='cmd.exe' AND ProcessId=" + processId;
      // Wait for the process to exit.
      dynamic watcher = Util.GetObject().ExecNotificationQuery(wmiQuery);
      dynamic cmdProcess = watcher.NextEvent();
      try
      {
        return cmdProcess.ExitStatus;
      }
      finally
      {
        Util.ReleaseComObject(ref cmdProcess);
        Util.ReleaseComObject(ref watcher);
      }
    }

    /// <summary>Request administrator privileges.</summary>
    static void RequestAdminPrivileges()
    {
      if (IsCurrentProcessElevated()) return;
      dynamic shell = Util.CreateObject("Shell.Application");
      shell.ShellExecute(Path, Interaction.Command(), Missing.Value, "runas", Constants.vbHidden);
      Util.ReleaseComObject(ref shell);
      Quit(0);
    }

    /// <summary>Check if the process is elevated.</summary>
    /// <returns>True if the running process is elevated, false otherwise.</returns>
    static bool IsCurrentProcessElevated()
    {
      const int HKU = unchecked((int)0x80000003);
      bool bGranted;
      Util.Registry.CheckAccess(HKU, @"S-1-5-19\Environment", Missing.Value, out bGranted);
      return bGranted;
    }

    /// <summary>Clean up and quit.</summary>
    /// <param name="exitCode">The exit code.</param>
    internal static void Quit(int exitCode)
    {
      Util.Dispose();
      GC.Collect();
      Environment.Exit(exitCode);
    }
  }
}