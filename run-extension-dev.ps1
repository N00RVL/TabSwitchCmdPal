# TabSwitch Extension - Development Mode Runner
# This script runs the Command Palette extension directly from the build output

Write-Host "Starting TabSwitch Extension in Development Mode..." -ForegroundColor Green
Write-Host ""

# Build the extension first
Write-Host "Building extension..." -ForegroundColor Yellow
$buildResult = dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Check if the executable exists
$exePath = "TabSwitchExtension\bin\x64\Release\net9.0-windows10.0.22000.0\win-x64\TabSwitchExtension.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "Extension executable not found at: $exePath" -ForegroundColor Red
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
