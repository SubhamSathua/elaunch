param (
    [string]$Version = "v1.0.0"
)

$ErrorActionPreference = "Stop"

$RootDir = $PSScriptRoot
$DistDir = Join-Path $RootDir "dist"

Write-Host "==> Packaging ELaunch Release $Version..." -ForegroundColor Cyan

# Ensure clean dist directory
if (Test-Path $DistDir) {
    Remove-Item -Path $DistDir -Recurse -Force
}
New-Item -ItemType Directory -Path $DistDir | Out-Null

# 1. Zip Portable (.NET 8 Runtime required)
$PortableDir = Join-Path $RootDir "publish\portable\win-x64"
$PortableZip = Join-Path $DistDir "elaunch-$Version-win-x64-portable.zip"
if (Test-Path $PortableDir) {
    Write-Host "Creating $PortableZip..." -ForegroundColor Green
    Compress-Archive -Path "$PortableDir\*" -DestinationPath $PortableZip -Force
} else {
    Write-Host "Warning: $PortableDir not found. Skipping portable zip." -ForegroundColor Yellow
}

# 2. Zip Standalone / Self-contained
$SelfContainedDir = Join-Path $RootDir "publish\selfcontained\win-x64"
$SelfContainedZip = Join-Path $DistDir "elaunch-$Version-win-x64-portable-standalone.zip"
if (Test-Path $SelfContainedDir) {
    Write-Host "Creating $SelfContainedZip..." -ForegroundColor Green
    Compress-Archive -Path "$SelfContainedDir\*" -DestinationPath $SelfContainedZip -Force
} else {
    Write-Host "Warning: $SelfContainedDir not found. Skipping standalone zip." -ForegroundColor Yellow
}

# 3. Copy Setup MSI (Portable)
$PortableMsi = Join-Path $RootDir "installer\Release\Portable\ELaunch-Portable.msi"
$SetupMsi = Join-Path $DistDir "elaunch-$Version-win-x64-setup.msi"
if (Test-Path $PortableMsi) {
    Write-Host "Copying $SetupMsi..." -ForegroundColor Green
    Copy-Item -Path $PortableMsi -Destination $SetupMsi -Force
} else {
    Write-Host "Warning: $PortableMsi not found. Build MSI first." -ForegroundColor Yellow
}

# 4. Copy Setup MSI (Standalone)
$SelfContainedMsi = Join-Path $RootDir "installer\Release\SelfContained\ELaunch-SelfContained.msi"
$SetupStandaloneMsi = Join-Path $DistDir "elaunch-$Version-win-x64-setup-standalone.msi"
if (Test-Path $SelfContainedMsi) {
    Write-Host "Copying $SetupStandaloneMsi..." -ForegroundColor Green
    Copy-Item -Path $SelfContainedMsi -Destination $SetupStandaloneMsi -Force
} else {
    Write-Host "Warning: $SelfContainedMsi not found. Build MSI first." -ForegroundColor Yellow
}

Write-Host "`n==> Packaging complete! Prepared assets in dist/ directory:" -ForegroundColor Cyan
Get-ChildItem -Path $DistDir | Select-Object Name, @{Name="Size (MB)"; Expression={[math]::Round($_.Length / 1MB, 2)}}
