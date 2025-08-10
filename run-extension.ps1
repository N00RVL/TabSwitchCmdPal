# TabSwitch Extension Direct Run Script
# This runs the extension directly without MSIX installation

Write-Host "Running TabSwitch Extension directly..." -ForegroundColor Green

# Install Native Host first
Write-Host "Installing Native Host..." -ForegroundColor Green
& "Setup\install.ps1"

# Build the extension
Write-Host "Building Command Palette Extension..." -ForegroundColor Green
dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64

# Run the extension directly
Write-Host "Starting Command Palette Extension..." -ForegroundColor Green
Write-Host "The extension will run in debug mode. Close this window to stop it." -ForegroundColor Yellow

$exePath = "TabSwitchExtension\bin\x64\Release\net9.0-windows10.0.22000.0\win-x64\TabSwitchExtension.exe"
if (Test-Path $exePath) {
    & $exePath
}
else {
    Write-Host "Extension executable not found. Please build the project first." -ForegroundColor Red
}
