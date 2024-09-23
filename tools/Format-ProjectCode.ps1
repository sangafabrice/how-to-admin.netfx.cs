<#PSScriptInfo .VERSION 1.0.0#>

[CmdletBinding()]
param ([string[]] $Path)
Get-ChildItem $Path -Recurse | ForEach-Object {
  $content = @(Get-Content $_.FullName).TrimEnd() -join [Environment]::NewLine
  Set-Content $_.FullName $content -NoNewLine
}