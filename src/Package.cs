/// <summary>Information about the resource files used by the project.</summary>
/// <version>0.0.1.0</version>

using System;
using System.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace cvmd2html
{
  static class Package
  {
    const string POWERSHELL_SUBKEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\pwsh.exe";

    /// <summary>The project root path</summary>
    static readonly string _root = AppContext.BaseDirectory;

    /// <summary>The powershell core runtime path.</summary>
    internal static readonly string PwshExePath = Registry.GetValue(POWERSHELL_SUBKEY, null, null) as string;

    /// <summary>The project resources directory path.</summary>
    static readonly string _resourcePath = Path.Combine(_root, "rsc");

    /// <summary>The shortcut target powershell script path.</summary>
    internal static readonly string PwshScriptPath = Path.Combine(_resourcePath, "cvmd2html.ps1");

    /// <summary>adapted link object.</summary>
    internal static class IconLink
    {
      /// <summary>The custom icon link full path.</summary>
      internal static readonly string Path = Util.GenerateRandomPath(".lnk");

      /// <summary>Create the custom icon link file.</summary>
      /// <param name="markdownPath">The input markdown file path.</param>
      internal static void Create(string markdownPath)
      {
        WshShell shell = new();
        var link = shell.CreateShortcut(Path) as WshShortcut;
        link.TargetPath = PwshExePath;
        link.Arguments = string.Format(@"-ep Bypass -nop -w Hidden -f ""{0}"" -Markdown ""{1}""", PwshScriptPath, markdownPath);
        link.IconLocation = Program.Path;
        link.Save();
        Util.ReleaseComObject(ref link);
        Util.ReleaseComObject(ref shell);
      }

      /// <summary>Delete the custom icon link file.</summary>
      internal static void Delete() => Util.DeleteFile(Path);
    }
  }
}