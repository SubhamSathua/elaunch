param (
    [string]$VersionOverride = ""
)

$ErrorActionPreference = "Stop"
$RootDir = $PSScriptRoot

# Clean any running processes that might lock publish files
Get-Process -Name "ELaunch" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

# 1. Read version from version.json or override
$VersionJsonPath = Join-Path $RootDir "version.json"
if (Test-Path $VersionJsonPath) {
    $VersionData = Get-Content $VersionJsonPath -Raw | ConvertFrom-Json
    $Version = if ($VersionOverride) { $VersionOverride } else { $VersionData.version }
} else {
    $Version = if ($VersionOverride) { $VersionOverride } else { "2.4.0" }
}

$TagVersion = "v$Version"
$DistDir = Join-Path $RootDir "dist"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  ELaunch Release Build & Package Pipeline ($TagVersion)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 2. Clean previous build output
Write-Host "`n[1/4] Cleaning previous output directories..." -ForegroundColor Yellow
if (Test-Path $DistDir) { Remove-Item -Path $DistDir -Recurse -Force }
New-Item -ItemType Directory -Path $DistDir | Out-Null

$PublishDir = Join-Path $RootDir "publish"
if (Test-Path $PublishDir) { Remove-Item -Path $PublishDir -Recurse -Force }

# 3. Dotnet Publish (Portable & Standalone)
Write-Host "`n[2/4] Publishing dotnet binaries..." -ForegroundColor Yellow

$PortableOut = Join-Path $RootDir "publish\portable\win-x64"
Write-Host " -> Publishing Portable build (.NET runtime required)..." -ForegroundColor Gray
dotnet publish "$RootDir\ELaunch\ELaunch.csproj" -c Release -r win-x64 --self-contained false -o $PortableOut
if ($LASTEXITCODE -ne 0) { throw "Dotnet publish portable failed." }

$StandaloneOut = Join-Path $RootDir "publish\selfcontained\win-x64"
Write-Host " -> Publishing Standalone build (Self-contained)..." -ForegroundColor Gray
dotnet publish "$RootDir\ELaunch\ELaunch.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o $StandaloneOut
if ($LASTEXITCODE -ne 0) { throw "Dotnet publish standalone failed." }

# 4. Build MSI Installers with Visual Studio devenv (if available)
Write-Host "`n[3/4] Building MSI installers..." -ForegroundColor Yellow
$DevenvPath = "E:\Programs\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe"
if (-not (Test-Path $DevenvPath)) {
    $VSWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $VSWhere) {
        $VSInstall = & $VSWhere -latest -property installationPath
        if ($VSInstall) {
            $DevenvPath = Join-Path $VSInstall "Common7\IDE\devenv.exe"
        }
    }
}

if (Test-Path $DevenvPath) {
    Write-Host " -> Found devenv at $DevenvPath. Building MSI solution..." -ForegroundColor Gray
    
    # Regenerate vdproj files if script is available
    $GenVdprojScript = "$env:LOCALAPPDATA\Temp\opencode\gen-vdproj.ps1"
    if (Test-Path $GenVdprojScript) {
        Write-Host " -> Regenerating .vdproj files with version $Version..." -ForegroundColor Gray
        & $GenVdprojScript -Name "SelfContained" -SourceDir $StandaloneOut -OutVdproj "$RootDir\installer\ELaunch-SelfContained.vdproj" -ProductVersion $Version
        & $GenVdprojScript -Name "Portable" -SourceDir $PortableOut -OutVdproj "$RootDir\installer\ELaunch-Portable.vdproj" -ProductVersion $Version
    }

    $SlnPath = Join-Path $RootDir "ELaunch-Installer.sln"
    & "$DevenvPath" "$SlnPath" /Rebuild "Release"
    
    # Wait for devenv file writing to complete cleanly
    $waitCounter = 0
    while ((Test-Path "$RootDir\installer\Release\SelfContained\ELaunch-SelfContained.tmp") -and ($waitCounter -lt 30)) {
        Start-Sleep -Seconds 1
        $waitCounter++
    }
} else {
    Write-Host "Notice: Visual Studio devenv.exe not found. Skipping MSI build." -ForegroundColor Yellow
}

# 5. Package assets into dist/
Write-Host "`n[4/4] Packaging release assets into dist/..." -ForegroundColor Yellow

# Portable ZIP
$PortableZip = Join-Path $DistDir "elaunch-$TagVersion-win-x64-portable.zip"
if (Test-Path $PortableOut) {
    Write-Host " -> Creating $PortableZip" -ForegroundColor Green
    Compress-Archive -Path "$PortableOut\*" -DestinationPath $PortableZip -Force
}

# Standalone ZIP
$StandaloneZip = Join-Path $DistDir "elaunch-$TagVersion-win-x64-portable-standalone.zip"
if (Test-Path $StandaloneOut) {
    Write-Host " -> Creating $StandaloneZip" -ForegroundColor Green
    Compress-Archive -Path "$StandaloneOut\*" -DestinationPath $StandaloneZip -Force
}

# Setup MSI (Portable)
$PortableMsiSource = Join-Path $RootDir "installer\Release\Portable\ELaunch-Portable.msi"
$SetupMsi = Join-Path $DistDir "elaunch-$TagVersion-win-x64-setup.msi"
if (Test-Path $PortableMsiSource) {
    Write-Host " -> Creating $SetupMsi" -ForegroundColor Green
    Copy-Item -Path $PortableMsiSource -Destination $SetupMsi -Force
}

# Setup MSI (Standalone)
$StandaloneMsiSource = Join-Path $RootDir "installer\Release\SelfContained\ELaunch-SelfContained.msi"
$SetupStandaloneMsi = Join-Path $DistDir "elaunch-$TagVersion-win-x64-setup-standalone.msi"
if (Test-Path $StandaloneMsiSource) {
    Write-Host " -> Creating $SetupStandaloneMsi" -ForegroundColor Green
    Copy-Item -Path $StandaloneMsiSource -Destination $SetupStandaloneMsi -Force
} else {
    Write-Host "Notice: Standalone MSI not produced by devenv; copying Portable MSI fallback or skipping." -ForegroundColor Yellow
}

Write-Host "`n==========================================================" -ForegroundColor Cyan
Write-Host "  SUCCESS! Release artifacts ready in dist/ folder:" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Get-ChildItem -Path $DistDir | Select-Object Name, @{Name="Size (MB)"; Expression={[math]::Round($_.Length / 1MB, 2)}}
