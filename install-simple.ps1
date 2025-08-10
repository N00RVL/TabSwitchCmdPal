Write-Host "=== TabSwitch Extension Installation ===" -ForegroundColor Green
Write-Host ""

# 1. Install Native Host
Write-Host "1. Installing Native Host..." -ForegroundColor Yellow
& "Setup\install.ps1"
Write-Host "✓ Native Host installation complete" -ForegroundColor Green

# 2. Build the Command Palette Extension
Write-Host ""
Write-Host "2. Building Command Palette Extension..." -ForegroundColor Yellow
dotnet build "TabSwitchExtension\TabSwitchExtension.csproj" -c Release -p:Platform=x64
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Command Palette Extension built successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to build Command Palette Extension" -ForegroundColor Red
}

# 3. Verify Native Host Registration
Write-Host ""
Write-Host "3. Verifying Installation..." -ForegroundColor Yellow
$chromeNativeHost = "$env:LOCALAPPDATA\Google\Chrome\User Data\NativeMessagingHosts\com.tabswitch.nativehost.json"

if (Test-Path $chromeNativeHost) {
    Write-Host "✓ Native Host registered for Chrome" -ForegroundColor Green
} else {
    Write-Host "✗ Native Host not registered for Chrome" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== INSTALLATION COMPLETE ===" -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Install Browser Extensions:" -ForegroundColor White
Write-Host "   Chrome: chrome://extensions/ -> Load unpacked -> BrowserExtensions/Chrome" -ForegroundColor Gray
Write-Host "   Firefox: about:debugging -> Load Temporary Add-on -> BrowserExtensions/Firefox/manifest.json" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Run the Extension:" -ForegroundColor White
Write-Host "   Run: .\run-extension-dev.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Test by opening Command Palette and typing 'Open Tab'" -ForegroundColor White
Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
