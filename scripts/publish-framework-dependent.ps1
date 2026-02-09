$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path "$PSScriptRoot\.."
$project = Join-Path $repoRoot "src\AvailabilityCompass.WpfClient\AvailabilityCompass.WpfClient.csproj"
$output = Join-Path $repoRoot "publish\framework-dependent"

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}
New-Item -ItemType Directory -Path $output | Out-Null

& dotnet publish $project -c Release -r win-x64 -o $output /p:SelfContained=false
