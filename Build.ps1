$ErrorActionPreference = "STOP"

function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [scriptblock]$cmd
    )
    
    & $cmd
    
    if ($LASTEXITCODE -ne 0) 
    {
        throw ("Error, exit code {0}" -f $LASTEXITCODE)
    }
}

# Cleanup
if(Test-Path .\artifacts) 
{
    Remove-Item .\artifacts -Force -Recurse
}

# Setup
$build = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $null]
$configuration = @{ $true = $env:CONFIGURATION; $false = "Release" }[$env:CONFIGURATION -ne $null]

# Restore
Exec { nuget restore }

# Build
Exec { msbuild (Get-Item "*.sln").FullName /t:Rebuild /p:Configuration=$configuration /v:m }

# Pack
New-Item -Path .\artifacts -ItemType Directory | Out-Null

$nuspecs = ".\SitecorePrometheusMetrics.nuspec"

$nuspecs | % {
    $version = ([xml](Get-Content -Path "$_")).package.metadata.version
    $final = $null
    
    if($version -match "-")
    {
        # Unstable
        $final = "{0}-{1}" -f $version, $build.ToString().PadLeft(5, "0")
    }
    else
    {
        # Stable
        $final = $version
    }

    Exec { nuget pack "$_" -OutputDirectory .\artifacts -Version $final }
}