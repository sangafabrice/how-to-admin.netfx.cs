/// <summary>Some utility methods.</summary>
/// <version>0.0.1.2</version>

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace cvmd2html
{
  static class Util
  {
    /// <summary>Generate a random file path.</summary>
    /// <param name="extension">The file extension.</param>
    /// <returns>A random file path.</returns>
    internal static string GenerateRandomPath(string extension) => Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tmp" + extension);

    /// <summary>Delete the specified file.</summary>
    /// <param name="filePath">The file path.</param>
    internal static void DeleteFile(string filePath)
    {
      try
      {
        File.Delete(filePath);
      }
      catch
      { }
    }

    /// <summary>Show the application message box.</summary>
    /// <param name="messageText">The message text to show.</param>
    /// <param name="popupType">The type of popup box.</param>
    /// <param name="popupButtons">The buttons of the message box.</param>
    internal static void Popup(string messageText, MessageBoxImage popupType = MessageBoxImage.None, MessageBoxButton popupButtons = MessageBoxButton.OK) => MessageBox.Show(messageText, "Convert to HTML", popupButtons, popupType);

    /// <summary>Release the specified COM object.</summary>
    /// <param name="comObject">The COM object to destroy.</param>
    internal static void ReleaseComObject<T>(ref T comObject)
    {
      Marshal.FinalReleaseComObject(comObject);
      comObject = default;
    }
  }
}