<#PSScriptInfo .VERSION 1.0.1#>

[CmdletBinding()]
param (
  [string] $Root,
  [string] $DirName,
  [string] $ClassName
)

$FileName = $ClassName.Replace('_', '.')
$EnvPath = $Env:Path
$Env:Path = "$Env:windir\Microsoft.NET\Framework$(If ([Environment]::Is64BitOperatingSystem) { '64' })\v4.0.30319\;$Env:Path"
csc.exe /nologo /target:library /out:$(($ClassDll = "$DirName\$FileName.dll")) "$(($SrcDir = "$Root\src"))\AssemblyInfo.cs" "$SrcDir\$FileName.cs"
$Env:Path = $EnvPath
return $ClassDll