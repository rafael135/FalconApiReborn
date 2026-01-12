# Builds solution and collects XML documentation files into docs/api
param(
    [string]$Configuration = 'Release'
)

Write-Host "Building solution in $Configuration..."
dotnet build -c $Configuration

$xmls = Get-ChildItem -Path './src' -Recurse -Include '*.xml' | Where-Object { $_.FullName -match '\\bin\\' }

if (-not $xmls) {
    Write-Error "No XML documentation files found. Make sure projects have <GenerateDocumentationFile>true</GenerateDocumentationFile> set and build succeeded."
    exit 1
}

$dest = Join-Path -Path 'docs' -ChildPath 'api'
if (-not (Test-Path $dest)) { New-Item -ItemType Directory -Path $dest | Out-Null }

foreach ($xml in $xmls) {
    Write-Host "Copying $($xml.FullName) -> $dest"
    Copy-Item -Path $xml.FullName -Destination $dest -Force
}

Write-Host "Done. XML documentation files copied to $dest."