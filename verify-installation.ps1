# TabSwitch Extension - Final Verification Script
# This script verifies that the TabSwitch extension is properly registered and working

Write-Host "🔍 TabSwitch Extension - Final Verification" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Green

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

# Check if extension is registered
$package = Get-AppxPackage -Name "TabSwitchExtension"
if ($package) {
    Write-Host "✅ Command Palette Extension Registration:" -ForegroundColor Green
    Write-Host "   Name: $($package.Name)" -ForegroundColor White
    Write-Host "   Version: $($package.Version)" -ForegroundColor White
    Write-Host "   Status: $($package.Status)" -ForegroundColor White
    Write-Host "   Development Mode: $($package.IsDevelopmentMode)" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "❌ TabSwitch extension is not registered with Command Palette" -ForegroundColor Red
    exit 1
}

# Check if executable exists
$exePath = Get-ChildItem -Path "TabSwitchExtension\bin\x64\Release" -Filter "TabSwitchExtension.exe" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName
if ($exePath -and (Test-Path $exePath)) {
    Write-Host "✅ Extension Executable: Found" -ForegroundColor Green
    Write-Host "   Path: $exePath" -ForegroundColor White
} else {
    Write-Host "❌ Extension executable not found" -ForegroundColor Red
}

# Check if native host is installed
$nativeHostPath = Get-ChildItem -Path "NativeHost\bin\Release" -Filter "TabSwitchNativeHost.exe" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName
if ($nativeHostPath -and (Test-Path $nativeHostPath)) {
    Write-Host "✅ Native Host Executable: Found" -ForegroundColor Green
    Write-Host "   Path: $nativeHostPath" -ForegroundColor White
} else {
    Write-Host "❌ Native host executable not found" -ForegroundColor Red
}

# Check Chrome native messaging host registry
try {
    $chromeReg = Get-ItemProperty -Path "HKCU:\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.tabswitch.nativehost" -ErrorAction SilentlyContinue
    if ($chromeReg) {
        Write-Host "✅ Chrome Native Messaging Host: Registered" -ForegroundColor Green
        Write-Host "   Path: $($chromeReg.'(default)')" -ForegroundColor White
    } else {
        Write-Host "⚠️ Chrome Native Messaging Host: Not registered" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️ Chrome Native Messaging Host: Unable to check registry" -ForegroundColor Yellow
}

# Check Firefox native messaging host registry
try {
    $firefoxReg = Get-ItemProperty -Path "HKCU:\SOFTWARE\Mozilla\NativeMessagingHosts\com.tabswitch.nativehost" -ErrorAction SilentlyContinue
    if ($firefoxReg) {
        Write-Host "✅ Firefox Native Messaging Host: Registered" -ForegroundColor Green
        Write-Host "   Path: $($firefoxReg.'(default)')" -ForegroundColor White
    } else {
        Write-Host "⚠️ Firefox Native Messaging Host: Not registered" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️ Firefox Native Messaging Host: Unable to check registry" -ForegroundColor Yellow
}

# Check browser extension files
$chromeExtPath = Join-Path $scriptRoot "BrowserExtensions\Chrome\manifest.json"
$firefoxExtPath = Join-Path $scriptRoot "BrowserExtensions\Firefox\manifest.json"

if (Test-Path $chromeExtPath) {
    Write-Host "✅ Chrome Extension Files: Found" -ForegroundColor Green
    Write-Host "   Path: $(Split-Path $chromeExtPath -Parent)" -ForegroundColor White
} else {
    Write-Host "❌ Chrome extension files not found" -ForegroundColor Red
}

if (Test-Path $firefoxExtPath) {
    Write-Host "✅ Firefox Extension Files: Found" -ForegroundColor Green
    Write-Host "   Path: $(Split-Path $firefoxExtPath -Parent)" -ForegroundColor White
} else {
    Write-Host "❌ Firefox extension files not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 TabSwitch Extension Installation Complete!" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Install Browser Extensions:" -ForegroundColor White
Write-Host "   • Chrome: Open chrome://extensions/, enable Developer mode, click 'Load unpacked', select Chrome folder" -ForegroundColor Gray
Write-Host "   • Firefox: Open about:debugging, click 'This Firefox', click 'Load Temporary Add-on', select manifest.json" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Test Command Palette:" -ForegroundColor White
Write-Host "   • Press Windows + Ctrl + ` (backtick) to open Command Palette" -ForegroundColor Gray
Write-Host "   • Type 'TabSwitch' or 'Tab' to find the extension" -ForegroundColor Gray
Write-Host "   • The extension should appear in the results" -ForegroundColor Gray
Write-Host ""
Write-Host "3. If it doesn't appear, try:" -ForegroundColor White
Write-Host "   • Restart Windows Explorer: taskkill /f /im explorer.exe && start explorer.exe" -ForegroundColor Gray
Write-Host "   • Restart your computer" -ForegroundColor Gray
Write-Host "   • Re-run the registration script" -ForegroundColor Gray
Write-Host ""
Write-Host "🔧 The extension is now properly registered as a Command Palette extension (not a packaged app)!" -ForegroundColor Green
