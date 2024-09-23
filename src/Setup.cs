/// <summary>The methods for managing the shortcut menu option: install and uninstall.</summary>
/// <version>0.0.1.0</version>

using System;
using Microsoft.Win32;

namespace cvmd2html
{
  static class Setup
  {
    const string SHELL_SUBKEY = @"SOFTWARE\Classes\SystemFileAssociations\.md\shell";
    const string VERB = "cthtml";
    static readonly string VERB_SUBKEY = String.Format(@"{0}\{1}", SHELL_SUBKEY, VERB);
    static readonly string VERB_KEY = String.Format(@"{0}\{1}", Registry.CurrentUser, VERB_SUBKEY);
    const string ICON_VALUENAME = "Icon";

    /// <summary>Configure the shortcut menu in the registry.</summary>
    internal static void Set()
    {
      string COMMAND_KEY = VERB_KEY + @"\command";
      var command = String.Format(@"""{0}"" /Markdown:""%1""", Program.Path);
      Registry.SetValue(COMMAND_KEY, null, command);
      Registry.SetValue(VERB_KEY, null, "Convert to &HTML");
    }

    /// <summary>Add an icon to the shortcut menu in the registry.</summary>
    internal static void AddIcon()
    {
      Registry.SetValue(VERB_KEY, ICON_VALUENAME, Program.Path);
    }

    /// <summary>Remove the shortcut icon menu.</summary>
    internal static void RemoveIcon()
    {
      using (RegistryKey VERB_KEY_OBJ = Registry.CurrentUser.OpenSubKey(VERB_SUBKEY, true))
      {
        if (VERB_KEY_OBJ != null)
        {
          VERB_KEY_OBJ.DeleteValue(ICON_VALUENAME, false);
        }
      }
    }

    /// <summary>Remove the shortcut menu by removing the verb key and subkeys.</summary>
    internal static void Unset()
    {
      using (RegistryKey SHELL_KEY_OBJ = Registry.CurrentUser.OpenSubKey(SHELL_SUBKEY, true))
      {
        if (SHELL_KEY_OBJ != null)
        {
          SHELL_KEY_OBJ.DeleteSubKeyTree(VERB, false);
        }
      }
    }
  }
}