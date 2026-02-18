# TabSwitch Extension - Development Mode Runner
# This script runs the Command Palette extension directly from the build output

Write-Host "Starting TabSwitch Extension in Development Mode..." -ForegroundColor Green
Write-Host ""

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

# Build the extension first
Write-Host "Building extension..." -ForegroundColor Yellow
$buildResult = dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Check if the executable exists
$exePath = Get-ChildItem -Path "TabSwitchExtension\bin\x64\Release" -Filter "TabSwitchExtension.exe" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName
if (-not (Test-Path $exePath)) {
    Write-Host "Extension executable not found under TabSwitchExtension\bin\x64\Release" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Extension built successfully" -ForegroundColor Green
Write-Host ""
Write-Host "Starting extension..." -ForegroundColor Yellow
Write-Host "The extension will run in development mode." -ForegroundColor Gray
Write-Host "Close this window or press Ctrl+C to stop the extension." -ForegroundColor Gray
Write-Host ""

# Run the extension
try {
    & $exePath
}
catch {
    Write-Host "Failed to start extension: $_" -ForegroundColor Red
    exit 1
}
