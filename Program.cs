/// <summary>Launch the shortcut's target PowerShell script with the markdown.</summary>
/// <version>0.0.1.4</version>

using System;
using System.Diagnostics;
using System.Reflection;
using ROOT.CIMV2;
using WbemScripting;
using System.ComponentModel;

[assembly: AssemblyTitle("CvMd2Html")]

namespace cvmd2html
{
  static class Program
  {
    static void Main(string[] args)
    {
      RequestAdminPrivileges(args);

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
      var wbemLocator = new SWbemLocator();
      SWbemServices wmiService = wbemLocator.ConnectServer();
      SWbemEventSource watcher = wmiService.ExecNotificationQuery(wmiQuery);
      SWbemObject cmdProcess = watcher.NextEvent();
      try
      {
        return (int)cmdProcess.Properties_.Item("ExitStatus").get_Value();
      }
      finally
      {
        _.ReleaseComObject(ref cmdProcess);
        _.ReleaseComObject(ref watcher);
        _.ReleaseComObject(ref wmiService);
        _.ReleaseComObject(ref wbemLocator);
      }
    }

    /// <summary>Request administrator privileges.</summary>
    /// <param name="args">The command line arguments.</param>
    static void RequestAdminPrivileges(string[] args)
    {
      if (IsCurrentProcessElevated()) return;
      try
      {
        Process.Start(
          new ProcessStartInfo(Path, String.Format(@"""{0}""", String.Join(@""" """, args)))
          {
            UseShellExecute = true,
            Verb = "runas",
            WindowStyle = ProcessWindowStyle.Hidden
          }
        );
      }
      catch (Win32Exception)
      {
        Quit(0);
      }
      catch (Exception)
      {
        Quit(1);
      }
      Quit(0);
    }

    /// <summary>Check if the process is elevated.</summary>
    /// <returns>True if the running process is elevated, false otherwise.</returns>
    static bool IsCurrentProcessElevated()
    {
      const uint HKU = 0x80000003;
      bool bGranted;
      StdRegProv.CheckAccess(HKU, @"S-1-5-19\Environment", out bGranted);
      return bGranted;
    }

    /// <summary>Clean up and quit.</summary>
    /// <param name="exitCode">The exit code.</param>
    internal static void Quit(int exitCode)
    {
      GC.Collect();
      Environment.Exit(exitCode);
    }
  }
}