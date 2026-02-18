# TabSwitch Extension Installation Script
# Run this script as Administrator after enabling Developer Mode

Write-Host "Installing TabSwitch Extension..." -ForegroundColor Green

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

# Check if developer mode is enabled
$devModeKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock"
$devMode = $false
try {
    $devMode = (Get-ItemProperty -Path $devModeKey -Name "AllowDevelopmentWithoutDevLicense" -ErrorAction SilentlyContinue).AllowDevelopmentWithoutDevLicense
}
catch {
    Write-Host "Could not check developer mode status" -ForegroundColor Yellow
}

if ($devMode -eq 1) {
    Write-Host "Developer mode is enabled. Installing MSIX package..." -ForegroundColor Green
    
    # Install the MSIX package
    $msix = Get-ChildItem -Path "TabSwitchExtension\bin" -Filter "*.msix" -Recurse -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
    if (-not $msix) {
        Write-Host "No MSIX package found under TabSwitchExtension\bin. Build/package the app first." -ForegroundColor Red
        exit 1
    }
    $msixPath = $msix.FullName
    
    try {
        Add-AppxPackage -Path $msixPath -AllowUnsigned
        Write-Host "Command Palette Extension installed successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to install Command Palette Extension: $_" -ForegroundColor Red
        Write-Host "Trying alternative installation method..." -ForegroundColor Yellow
        
        # Alternative: Install dependencies first
        $depsPath = Join-Path (Split-Path $msixPath -Parent) "Dependencies\x64"
        if (Test-Path $depsPath) {
            Get-ChildItem -Path $depsPath -Filter "*.appx" | ForEach-Object {
                try {
                    Add-AppxPackage -Path $_.FullName -AllowUnsigned
                    Write-Host "Installed dependency: $($_.Name)" -ForegroundColor Green
                }
                catch {
                    Write-Host "Failed to install dependency $($_.Name): $_" -ForegroundColor Yellow
                }
            }
        }
        
        # Try installing the main package again
        Add-AppxPackage -Path $msixPath -AllowUnsigned
        Write-Host "Command Palette Extension installed successfully!" -ForegroundColor Green
    }
}
else {
    Write-Host "Developer mode is not enabled. Please enable it first:" -ForegroundColor Red
    Write-Host "1. Open Settings > Update & Security > For developers" -ForegroundColor Yellow
    Write-Host "2. Select 'Developer mode'" -ForegroundColor Yellow
    Write-Host "3. Confirm the installation" -ForegroundColor Yellow
    Write-Host "4. Run this script again" -ForegroundColor Yellow
    return
}

# Install Native Host
Write-Host "Installing Native Host..." -ForegroundColor Green
& "Setup\install.ps1"

# Verify installation
Write-Host "Verifying installation..." -ForegroundColor Green
$installed = Get-AppxPackage | Where-Object { $_.Name -like "*TabSwitch*" }
if ($installed) {
    Write-Host "✓ Command Palette Extension installed: $($installed.Name)" -ForegroundColor Green
}
else {
    Write-Host "✗ Command Palette Extension not found in installed packages" -ForegroundColor Red
}

# Check native host registration
$nativeHostPath = "$env:LOCALAPPDATA\Google\Chrome\User Data\NativeMessagingHosts\com.tabswitch.nativehost.json"
if (Test-Path $nativeHostPath) {
    Write-Host "✓ Native Host registered for Chrome" -ForegroundColor Green
}
else {
    Write-Host "✗ Native Host not registered for Chrome" -ForegroundColor Red
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Install browser extensions:" -ForegroundColor White
Write-Host "   Chrome: chrome://extensions/ -> Load unpacked -> BrowserExtensions/Chrome" -ForegroundColor Gray
Write-Host "   Firefox: about:debugging -> Load Temporary Add-on -> BrowserExtensions/Firefox/manifest.json" -ForegroundColor Gray
Write-Host "2. Open Command Palette (Win+R or configured hotkey)" -ForegroundColor White
Write-Host "3. Type 'Open Tab' to search browser tabs" -ForegroundColor White
Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
