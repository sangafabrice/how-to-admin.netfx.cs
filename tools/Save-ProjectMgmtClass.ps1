<#PSScriptInfo .VERSION 1.0.1#>

[CmdletBinding()]
param ([string] $Root, [string] $DirName, [string] $ClassName)

$FileName = $ClassName.Replace('_', '.')
$EnvPath = $Env:Path
$Env:Path = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\Roslyn\;$Env:Path"
csc.exe /nologo /target:library /out:$(($ClassDll = "$DirName\$FileName.dll")) "$(($SrcDir = "$Root\src"))\AssemblyInfo.cs" "$SrcDir\$FileName.cs"
$Env:Path = $EnvPath
return $ClassDll