<#PSScriptInfo .VERSION 1.0.4#>

using namespace System.Management.Automation
[CmdletBinding()]
Param ()

& {
  Remove-Item ($LibDir = "$PSScriptRoot\lib") -Recurse -ErrorAction SilentlyContinue
  [void] (New-Item $LibDir -ItemType Directory -ErrorAction Stop)

  Function ImportTypeLibrary {
    <#
    .DESCRIPTION
    This function imports the specified type library.
    .NOTES
    This function must be called after initializing $BinDir
    .PARAMETER TypeLibPath
    The specified type library path.
    .PARAMETER Namespace
    The namespace used for imports in the AssemblyInfo.
    #>
    Param (
      [string] $TypeLibPath,
      [string] $Namespace
    )
    & "$PSScriptRoot\TlbImp.exe" /nologo /silent $TypeLibPath /out:"$LibDir\Interop.$Namespace.dll" /namespace:$Namespace
  }

  ImportTypeLibrary 'C:\Windows\System32\wshom.ocx' 'IWshRuntimeLibrary'

  Get-ChildItem "$LibDir\*" -Directory | Remove-Item -Recurse -Force
  'PresentationFramework','WindowsBase','PresentationCore' |
  ForEach-Object {
    Copy-Item "$Env:windir\Microsoft.NET\Framework$(If ([Environment]::Is64BitOperatingSystem) { '64' })\v4.0.30319\WPF\${_}.dll" $LibDir
  }
}