<#PSScriptInfo .VERSION 1.0.3#>

using namespace System.Management.Automation
[CmdletBinding()]
Param ()

& {
  Remove-Item ($LibDir = "$PSScriptRoot\lib") -Recurse -ErrorAction SilentlyContinue
  [void] (New-Item $LibDir -ItemType Directory -ErrorAction Stop)

  Function ImportMgmtClass {
    <#
    .DESCRIPTION
    This function compiles a Management Class' source code to dll libraries.
    .NOTES
    It must be called after getting the library Interop.IWshRuntimeLibrary.dll.
    It must also be called after the csc.exe path is added to PATH environment variable.
    <Class_Name> means the name of the class may have an underscore '_'
    which is replaced with '.' in the source code base name <Class.Name>.
    .PARAMETER ClassName
    The specified Management class name.
    #>
    Param (
      [string] $ClassName
    )
    $EnvPath = $Env:Path
    $Env:Path = "$Env:windir\Microsoft.NET\Framework$(If ([Environment]::Is64BitOperatingSystem) { '64' })\v4.0.30319\;$Env:Path"
    $FileName = $ClassName.Replace('_', '.')
    csc.exe /nologo /target:library /out:"$LibDir\$FileName.dll" "$(($SrcDir = "$PSScriptRoot\src"))\AssemblyInfo.cs" "$SrcDir\$FileName.cs"
    $Env:Path = $EnvPath
  }

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

  ImportTypeLibrary 'C:\Windows\System32\wbem\wbemdisp.tlb' 'WbemScripting'
  ImportTypeLibrary 'C:\Windows\System32\wshom.ocx' 'IWshRuntimeLibrary'
  ImportMgmtClass StdRegProv

  Get-ChildItem "$LibDir\*" -Directory | Remove-Item -Recurse -Force
  'PresentationFramework','WindowsBase','PresentationCore' |
  ForEach-Object {
    Copy-Item "$Env:windir\Microsoft.NET\Framework$(If ([Environment]::Is64BitOperatingSystem) { '64' })\v4.0.30319\WPF\${_}.dll" $LibDir
  }
}