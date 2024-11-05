<#PSScriptInfo .VERSION 1.0.0#>

[CmdletBinding()]
Param ()

git add *.cs *.ps1 *.csproj

& {
  # Set the version of the executable
  $infoFilePath = "$PSScriptRoot\src\AssemblyInfo.cs"
  $diff = (git diff HEAD *.cs | ForEach-Object { switch ($_[0]) { "+"{1} "-"{-1} } } | Measure-Object -Sum).Sum
  $version = ($diff -eq 0 ? 1:[Math]::Abs($diff)) + ([version](@(git show HEAD $infoFilePath)[-2] -split '"')[1]).Revision
  $matchRegex = '"((\d+\.){3})\d+"'
  $infoContent = (Get-Content $infoFilePath | ForEach-Object { if ($_ -match $matchRegex) { $_ -replace $matchRegex,"`"`${1}$version`"" } else { $_ } }) -join [Environment]::NewLine
  Set-Content $infoFilePath $infoContent -NoNewline
}

& {
  # Set the version of the source files
  git status -s *.cs |
  ConvertFrom-StringData -Delimiter " " |
  Where-Object { $_.Keys[0].EndsWith('M') } |
  ForEach-Object { $_.Values } |
  Where-Object { $_ -ne 'src/AssemblyInfo.cs' } |
  ForEach-Object {
    $version = git cat-file -p HEAD:$_ 2>&1 |
      Where-Object { $_ -match '/// <version>\d+(\.\d+){2,3}</version>' } |
      ForEach-Object { ([version]($_.TrimEnd().Substring('/// <version>'.Length) -split '<')[0]).Revision + 1 } |
      Select-Object -Last 1
    if (-not [String]::IsNullOrWhiteSpace($version)) {
      $content = (Get-Content $_ -Raw) -replace '/// <version>(\d+(\.\d+){2})(\.\d+)?</version>',"/// <version>`$1.$version</version>"
      Set-Content "$PSScriptRoot\$_" $content -NoNewline
    }
  }
}

& {
  # Set the version of the ps1 build files
  git status -s *.ps1 |
  ConvertFrom-StringData -Delimiter " " |
  Where-Object { $_.Keys[0].EndsWith('M') } |
  ForEach-Object { $_.Values } |
  Where-Object { $_ -notlike 'rsc/*.ps1' } |
  ForEach-Object {
    $version = git cat-file -p HEAD:$_ 2>&1 |
      Where-Object { $_ -match '<#PSScriptInfo .VERSION (\d+\.){2}\d+#>' } |
      ForEach-Object { ([version]($_.TrimEnd().Substring('<#PSScriptInfo .VERSION '.Length) -split '#')[0]).Build + 1 } |
      Select-Object -Last 1
    if (-not [String]::IsNullOrWhiteSpace($version)) {
      $content = (Get-Content $_ -Raw) -replace '<#PSScriptInfo .VERSION ((\d+\.){2})\d+#>',"<#PSScriptInfo .VERSION `${1}$version#>"
      Set-Content "$PSScriptRoot\$_" $content -NoNewline
    }
  }
}

& {
  # Set the version of the project files
  git status -s *.csproj |
  ConvertFrom-StringData -Delimiter " " |
  Where-Object { $_.Keys[0].EndsWith('M') } |
  ForEach-Object { $_.Values } |
  ForEach-Object {
    $version = git cat-file -p HEAD:$_ 2>&1 |
      Where-Object { $_ -match '<!\-\- (\d+\.){3}\d+ \-\->' } |
      ForEach-Object { ([version]($_.TrimEnd().Substring('<!-- '.Length) -split ' ')[0]).Revision + 1 } |
      Select-Object -Last 1
    if (-not [String]::IsNullOrWhiteSpace($version)) {
      $content = (Get-Content $_ -Raw) -replace '<!\-\- ((\d+\.){3})\d+ \-\->',"<!-- `${1}$version -->"
      Set-Content "$PSScriptRoot\$_" $content -NoNewline
    }
  }
}