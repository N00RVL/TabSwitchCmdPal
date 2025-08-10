# TabSwitch Extension - Simple Installation and Launch Script
# This script installs the native host and runs the Command Palette extension directly

Write-Host "=== TabSwitch Extension Installation ===" -ForegroundColor Green
Write-Host ""

# 1. Install Native Host
Write-Host "1. Installing Native Host..." -ForegroundColor Yellow
try {
    & "Setup\install.ps1"
    Write-Host "✓ Native Host installed successfully" -ForegroundColor Green
}
catch {
    Write-Host "✗ Failed to install Native Host: $_" -ForegroundColor Red
}

# 2. Build the Command Palette Extension
Write-Host ""
Write-Host "2. Building Command Palette Extension..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Command Palette Extension built successfully" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Failed to build Command Palette Extension" -ForegroundColor Red
        return
    }
}
catch {
    Write-Host "✗ Failed to build Command Palette Extension: $_" -ForegroundColor Red
    return
}

# 3. Verify Native Host Registration
Write-Host ""
Write-Host "3. Verifying Installation..." -ForegroundColor Yellow
$chromeNativeHost = "$env:LOCALAPPDATA\Google\Chrome\User Data\NativeMessagingHosts\com.tabswitch.nativehost.json"
$firefoxNativeHost = "$env:LOCALAPPDATA\Mozilla\NativeMessagingHosts\com.tabswitch.nativehost.json"

if (Test-Path $chromeNativeHost) {
    Write-Host "✓ Native Host registered for Chrome" -ForegroundColor Green
}
else {
    Write-Host "✗ Native Host not registered for Chrome" -ForegroundColor Red
}

if (Test-Path $firefoxNativeHost) {
    Write-Host "✓ Native Host registered for Firefox" -ForegroundColor Green
}
else {
    Write-Host "✗ Native Host not registered for Firefox" -ForegroundColor Red
}

# 4. Check if Command Palette for Windows is installed
Write-Host ""
Write-Host "4. Checking Command Palette for Windows..." -ForegroundColor Yellow
$cmdPalette = Get-Process "CommandPalette" -ErrorAction SilentlyContinue
if ($cmdPalette) {
    Write-Host "✓ Command Palette for Windows is running" -ForegroundColor Green
}
else {
    Write-Host "! Command Palette for Windows is not running" -ForegroundColor Yellow
    Write-Host "  You may need to install or start Command Palette for Windows" -ForegroundColor Gray
}

# 5. Installation Instructions
Write-Host ""
Write-Host "=== INSTALLATION COMPLETE ===" -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Install Browser Extensions:" -ForegroundColor White
Write-Host "   Chrome:" -ForegroundColor Cyan
Write-Host "   - Open chrome://extensions/" -ForegroundColor Gray
Write-Host "   - Enable 'Developer mode'" -ForegroundColor Gray
Write-Host "   - Click 'Load unpacked'" -ForegroundColor Gray
Write-Host "   - Select: D:\CmdPalExt\TabSwitchExtension\BrowserExtensions\Chrome" -ForegroundColor Gray
Write-Host ""
Write-Host "   Firefox:" -ForegroundColor Cyan
Write-Host "   - Open about:debugging" -ForegroundColor Gray
Write-Host "   - Click 'This Firefox'" -ForegroundColor Gray
Write-Host "   - Click 'Load Temporary Add-on'" -ForegroundColor Gray
Write-Host "   - Select: D:\CmdPalExt\TabSwitchExtension\BrowserExtensions\Firefox\manifest.json" -ForegroundColor Gray
Write-Host ""

Write-Host "2. Install Command Palette Extension:" -ForegroundColor White
Write-Host "   Option A - Run directly (Development mode):" -ForegroundColor Cyan
Write-Host "   - Run: .\run-extension-dev.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "   Option B - Install as MSIX (if you have a code signing certificate):" -ForegroundColor Cyan
Write-Host "   - Sign the MSIX package with your certificate" -ForegroundColor Gray
Write-Host "   - Install using: Add-AppxPackage -Path '...\TabSwitchExtension_0.0.1.0_x64.msix'" -ForegroundColor Gray
Write-Host ""

Write-Host "3. Test the Extension:" -ForegroundColor White
Write-Host "   - Open some browser tabs" -ForegroundColor Gray
Write-Host "   - Open Command Palette (Win+R or configured hotkey)" -ForegroundColor Gray
Write-Host "   - Type 'Open Tab' or 'TabSwitch'" -ForegroundColor Gray
Write-Host "   - Search for a tab by typing part of the page title" -ForegroundColor Gray
Write-Host "   - Press Enter to switch to that tab" -ForegroundColor Gray
Write-Host ""

Write-Host "FILES CREATED:" -ForegroundColor Yellow
Write-Host "- Native Host: $chromeNativeHost" -ForegroundColor Gray
Write-Host "- Extension Build: TabSwitchExtension\bin\x64\Release\..." -ForegroundColor Gray
Write-Host "- Browser Extensions: BrowserExtensions\Chrome\ and BrowserExtensions\Firefox\" -ForegroundColor Gray
Write-Host ""
Write-Host "Installation script completed!" -ForegroundColor Green
