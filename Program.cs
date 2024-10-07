/// <summary>Launch the shortcut's target PowerShell script with the markdown.</summary>
/// <version>0.0.1.5</version>

using System;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Security.Principal;
using System.Management;

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
    static uint WaitForExit(int processId)
    {
      // The process termination event query. Win32_ProcessStopTrace requires admin rights to be used.
      var wmiQuery = "SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName='cmd.exe' AND ProcessId=" + processId;
      // Wait for the process to exit.
      return (uint)new ManagementEventWatcher(wmiQuery).WaitForNextEvent().Properties["ExitStatus"].Value;
    }

    /// <summary>Request administrator privileges.</summary>
    /// <param name="args">The command line arguments.</param>
    static void RequestAdminPrivileges(string[] args)
    {
      if (IsCurrentProcessElevated()) return;
      try
      {
        Process.Start(
          new ProcessStartInfo(Path, args.Length > 0 ? String.Format(@"""{0}""", String.Join(@""" """, args)):"")
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
      return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
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