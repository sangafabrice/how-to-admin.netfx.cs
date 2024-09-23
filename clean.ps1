<#PSScriptInfo .VERSION 1.0.0#>

[CmdletBinding()]
Param ()

& {
  Get-ChildItem "$PSScriptRoot\*.cs","$PSScriptRoot\*.ps1","$PSScriptRoot\.gitignore" -Recurse | ForEach-Object {
    $content = @(Get-Content $_.FullName).TrimEnd() -join [Environment]::NewLine
    Set-Content $_.FullName $content -NoNewLine
  }
}