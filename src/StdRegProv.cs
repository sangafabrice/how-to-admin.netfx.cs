/// <summary>StdRegProv WMI class as inspired by mgmclassgen.exe.</summary>
/// <version>0.0.1.0</version>

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using WbemScripting;

[assembly: AssemblyTitle("StdRegProv")]

namespace ROOT.CIMV2
{
  public static class StdRegProv
  {
    static readonly string CreatedClassName = GetTypeName();

    public static uint CheckAccess(uint hDefKey, string sSubKeyName, out bool bGranted)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).CheckAccess(unchecked((int)hDefKey), sSubKeyName, Missing.Value, out bGranted);
      }
    }

    public static uint CreateKey(uint hDefKey, string sSubKeyName)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).CreateKey(unchecked((int)hDefKey), sSubKeyName);
      }
    }

    public static uint DeleteKey(uint hDefKey, string sSubKeyName)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).DeleteKey(unchecked((int)hDefKey), sSubKeyName);
      }
    }

    public static uint DeleteValue(uint hDefKey, string sSubKeyName, string sValueName = null)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).DeleteValue(unchecked((int)hDefKey), sSubKeyName, sValueName);
      }
    }

    public static uint EnumKey(uint hDefKey, string sSubKeyName, out string[] sNames)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        dynamic sNamesObj = null;
        var returnValue = (uint)(Registry.Provider as dynamic).EnumKey(unchecked((int)hDefKey), sSubKeyName, out sNamesObj);
        if (sNamesObj is DBNull)
        {
          sNames = new string[0];
        }
        else
        {
          Converter<object, string> cvobj2str = item => item.ToString();
          sNames = Array.ConvertAll(sNamesObj, cvobj2str);
        }
        return returnValue;
      }
    }

    public static uint GetStringValue(out string sValue, string sSubKeyName = null, string sValueName = null, uint? hDefKey = null)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).GetStringValue(hDefKey.HasValue ? (unchecked((int)hDefKey) as dynamic):Missing.Value, sSubKeyName, sValueName, out sValue);
      }
    }

    public static uint SetStringValue(uint hDefKey, string sSubKeyName, string sValue, string sValueName = null)
    {
      using (var Registry = new StdRegProvHelper(CreatedClassName))
      {
        return (uint)(Registry.Provider as dynamic).SetStringValue(unchecked((int)hDefKey), sSubKeyName, sValueName, sValue);
      }
    }

    /// <summary>Remove the key and all descendant subkeys.</summary>
    public static uint DeleteKeyTree(uint hDefKey, string sSubKeyName)
    {
      string[] sNames;
      uint returnValue = EnumKey(hDefKey, sSubKeyName, out sNames);
      if (sNames.Length > 0)
      {
        foreach (string sName in sNames)
        {
          returnValue += DeleteKeyTree(hDefKey, sSubKeyName + @"\" + sName);
        }
      }
      return returnValue += DeleteKey(hDefKey, sSubKeyName);
    }

    class StdRegProvHelper : IDisposable
    {
      SWbemLocator wbemLocator = new SWbemLocator();
      SWbemServices wbemService;
      internal SWbemObject Provider;

      internal StdRegProvHelper(string className)
      {
        wbemService = wbemLocator.ConnectServer();
        Provider = wbemService.Get(className);
      }

      /// <summary>Release the specified COM object.</summary>
      /// <param name="comObject">The COM object to destroy.</param>
      static void ReleaseComObject<T>(ref T comObject)
      {
        Marshal.FinalReleaseComObject(comObject);
        comObject = default(T);
      }

      public void Dispose()
      {
        ReleaseComObject(ref Provider);
        ReleaseComObject(ref wbemService);
        ReleaseComObject(ref wbemLocator);
      }
    }

    static string GetTypeName()
    {
      return new StackTrace().GetFrame(0).GetMethod().DeclaringType.Name;
    }
  }
}