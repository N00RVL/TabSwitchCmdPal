# TabSwitch Extension Direct Run Script
# This runs the extension directly without MSIX installation

Write-Host "Running TabSwitch Extension directly..." -ForegroundColor Green

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

# Install Native Host first
Write-Host "Installing Native Host..." -ForegroundColor Green
& "Setup\install.ps1"

# Build the extension
Write-Host "Building Command Palette Extension..." -ForegroundColor Green
dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64

# Run the extension directly
Write-Host "Starting Command Palette Extension..." -ForegroundColor Green
Write-Host "The extension will run in debug mode. Close this window to stop it." -ForegroundColor Yellow

$exePath = Get-ChildItem -Path "TabSwitchExtension\bin\x64\Release" -Filter "TabSwitchExtension.exe" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName

if ($exePath -and (Test-Path $exePath)) {
    & $exePath
}
else {
    Write-Host "Extension executable not found under TabSwitchExtension\bin\x64\Release. Please build the project first." -ForegroundColor Red
}
