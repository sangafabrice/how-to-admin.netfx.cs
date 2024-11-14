/// <summary>StdRegProv WMI class as inspired by mgmclassgen.exe.</summary>
/// <version>0.0.1.1</version>

using System;
using System.Management;
using System.Reflection;
using System.Diagnostics;

[assembly: AssemblyTitle("StdRegProv")]

namespace ROOT.CIMV2
{
  public static class StdRegProv
  {
    static readonly string CreatedClassName = GetTypeName();

    public static uint CheckAccess(uint hDefKey, string sSubKeyName, out bool bGranted)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      bGranted = (bool)outParams["bGranted"];
      return (uint)outParams["ReturnValue"];
    }

    public static uint CreateKey(uint hDefKey, string sSubKeyName)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      return (uint)outParams["ReturnValue"];
    }

    public static uint DeleteKey(uint hDefKey, string sSubKeyName)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      return (uint)outParams["ReturnValue"];
    }

    public static uint DeleteValue(uint hDefKey, string sSubKeyName, string sValueName = null)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      inParams["sValueName"] = sValueName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      return (uint)outParams["ReturnValue"];
    }

    public static uint EnumKey(uint hDefKey, string sSubKeyName, out string[] sNames)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      dynamic sNamesObj = outParams["sNames"];
      if (sNamesObj != null && sNamesObj.GetType().IsArray)
      {
        sNames = sNamesObj;
      }
      else
      {
        sNames = Array.Empty<string>();
      }
      return (uint)outParams["ReturnValue"];
    }

    public static uint GetStringValue(out string sValue, string sSubKeyName = null, string sValueName = null, uint? hDefKey = null)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      if (hDefKey.HasValue)
      {
        inParams["hDefKey"] = (uint)hDefKey;
      }
      inParams["sSubKeyName"] = sSubKeyName;
      inParams["sValueName"] = sValueName;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      sValue = outParams["sValue"] as string;
      return (uint)outParams["ReturnValue"];
    }

    public static uint SetStringValue(uint hDefKey, string sSubKeyName, string sValue, string sValueName = null)
    {
      string methodName = GetMethodName(new StackTrace());
      var classObj = new ManagementClass(CreatedClassName);
      ManagementBaseObject inParams = classObj.GetMethodParameters(methodName);
      inParams["hDefKey"] = hDefKey;
      inParams["sSubKeyName"] = sSubKeyName;
      inParams["sValueName"] = sValueName;
      inParams["sValue"] = sValue;
      ManagementBaseObject outParams = classObj.InvokeMethod(methodName, inParams, null);
      return (uint)outParams["ReturnValue"];
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

    static string GetTypeName()
    {
      return new StackTrace().GetFrame(0).GetMethod().DeclaringType.Name;
    }

    /// <summary>Get the name of the method calling this method.</summary>
    /// <param name="stackTrace">The stack trace from the calling method.</param>
    /// <returns>The name of the caller method.</returns>
    static string GetMethodName(StackTrace stackTrace)
    {
      return stackTrace.GetFrame(0).GetMethod().Name;
    }
  }
}