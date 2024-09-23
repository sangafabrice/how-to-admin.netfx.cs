/// <summary>ErrorLog manages the error log file and content.</summary>
/// <version>0.0.1.0</version>

using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace cvmd2html
{
  static class ErrorLog
  {
    /// <summary>The path to the generated error log file.</summary>
    internal static readonly string Path = Util.GenerateRandomPath(".log");

    /// <summary>Display the content of the error log file in a message box.</summary>
    internal static void Read()
    {
      try
      {
        using (StreamReader txtStream = File.OpenText(Path))
        {
          // Read the error message and remove the ANSI escaped character for red coloring.
          string errorMessage = Regex.Replace(txtStream.ReadToEnd(), @"(\x1B\[31;1m)|(\x1B\[0m)", "");
          if (errorMessage.Length > 0)
          {
            Util.Popup(errorMessage, MessageBoxImage.Error);
          }
        }
      }
      catch
      { }
    }

    /// <summary>Delete the error log file.</summary>
    internal static void Delete()
    {
      Util.DeleteFile(Path);
    }
  }
}