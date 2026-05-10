param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [string]$Platform = "AnyCPU",

    [string]$Target = "Restore,Build"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectGuid = "{4EC46DF5-2F3E-499E-A5A2-91FC44A5FF4E}"
$projectFile = Get-ChildItem -Path $repoRoot -Recurse -Filter "*.csproj" |
    Where-Object { (Get-Content -LiteralPath $_.FullName -Raw) -like "*$projectGuid*" } |
    Select-Object -First 1

if ($null -eq $projectFile) {
    throw "Project file not found for GUID $projectGuid."
}

$projectPath = $projectFile.FullName
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

if (-not (Test-Path -LiteralPath $vswhere)) {
    throw "vswhere.exe not found. Install Visual Studio Installer or Visual Studio Build Tools."
}

$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($msbuild) -or -not (Test-Path -LiteralPath $msbuild)) {
    throw "MSBuild.exe not found. Install the MSBuild component in Visual Studio Installer."
}

Write-Host "MSBuild: $msbuild"
Write-Host "Project: $projectPath"
Write-Host "Configuration: $Configuration"
Write-Host "Platform: $Platform"

& $msbuild $projectPath "/t:$Target" "/p:Configuration=$Configuration" "/p:Platform=$Platform" /m /v:minimal
exit $LASTEXITCODE
