# TabSwitch Extension Installation Script
# This script installs the Tab Switch Extension for Windows Command Palette

param(
    [switch]$Uninstall = $false
)

# Paths
$scriptDir = $PSScriptRoot
$solutionDir = Split-Path $scriptDir -Parent
$mainProjectPath = Join-Path $solutionDir "TabSwitchExtension\bin\x64\Debug\net9.0-windows10.0.22000.0\win-x64"
$nativeHostPath = Join-Path $solutionDir "NativeHost\bin\x64\Debug\net9.0-windows\win-x64\TabSwitchNativeHost.exe"
$chromeExtensionPath = Join-Path $solutionDir "BrowserExtensions\Chrome"
$firefoxExtensionPath = Join-Path $solutionDir "BrowserExtensions\Firefox"

# Native messaging host paths
$chromeNativeHostDir = "$env:USERPROFILE\AppData\Local\Google\Chrome\User Data\NativeMessagingHosts"
$firefoxNativeHostDir = "$env:USERPROFILE\AppData\Roaming\Mozilla\NativeMessagingHosts"

function Install-Extension {
    Write-Host "Installing TabSwitch Extension..." -ForegroundColor Green
    
    # 1. Build the projects
    Write-Host "Building projects..." -ForegroundColor Yellow
    Set-Location $solutionDir
    dotnet build TabSwitchExtension.sln -c Debug -p:Platform=x64
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. Please check the errors above."
        return
    }
    
    # 2. Install Command Palette Extension (MSIX package)
    Write-Host "Installing Command Palette Extension..." -ForegroundColor Yellow
    $msixPath = Get-ChildItem -Path $mainProjectPath -Filter "*.msix" | Select-Object -First 1
    if ($msixPath) {
        Add-AppxPackage -Path $msixPath.FullName -ForceApplicationShutdown
        Write-Host "Command Palette Extension installed successfully." -ForegroundColor Green
    } else {
        Write-Warning "MSIX package not found. You may need to package the app first."
    }
    
    # 3. Install Native Host
    Write-Host "Installing Native Host..." -ForegroundColor Yellow
    
    # Create directories
    New-Item -ItemType Directory -Path $chromeNativeHostDir -Force | Out-Null
    New-Item -ItemType Directory -Path $firefoxNativeHostDir -Force | Out-Null
    
    # Copy native host executable
    $nativeHostInstallPath = "$env:USERPROFILE\AppData\Local\TabSwitch\TabSwitchNativeHost.exe"
    New-Item -ItemType Directory -Path (Split-Path $nativeHostInstallPath) -Force | Out-Null
    Copy-Item $nativeHostPath $nativeHostInstallPath -Force
    
    # Create Chrome manifest
    $chromeManifest = @{
        name = "com.tabswitch.nativehost"
        description = "TabSwitch Native Host for Chrome"
        path = $nativeHostInstallPath
        type = "stdio"
        allowed_origins = @(
            "chrome-extension://tabswitchextension/"
        )
    } | ConvertTo-Json -Depth 10
    
    $chromeManifest | Out-File -FilePath "$chromeNativeHostDir\com.tabswitch.nativehost.json" -Encoding UTF8
    
    # Create Firefox manifest
    $firefoxManifest = @{
        name = "com.tabswitch.nativehost"
        description = "TabSwitch Native Host for Firefox"
        path = $nativeHostInstallPath.Replace('\', '\\')
        type = "stdio"
        allowed_extensions = @(
            "tabswitch@extension.com"
        )
        
    } | ConvertTo-Json -Depth 10
    
    $firefoxManifest | Out-File -FilePath "$firefoxNativeHostDir\com.tabswitch.nativehost.json" -Encoding UTF8
    
    Write-Host "Native Host installed successfully." -ForegroundColor Green
    
    # 4. Browser Extension Instructions
    Write-Host "`nBrowser Extension Installation:" -ForegroundColor Cyan
    Write-Host "Chrome:" -ForegroundColor Yellow
    Write-Host "  1. Open chrome://extensions/"
    Write-Host "  2. Enable 'Developer mode'"
    Write-Host "  3. Click 'Load unpacked'"
    Write-Host "  4. Select folder: $chromeExtensionPath" 
    
    Write-Host "`nFirefox:" -ForegroundColor Yellow
    Write-Host "  1. Open about:debugging"
    Write-Host "  2. Click 'This Firefox'"
    Write-Host "  3. Click 'Load Temporary Add-on'"
    Write-Host "  4. Select manifest.json from: $firefoxExtensionPath"
    
    Write-Host "`nInstallation completed successfully!" -ForegroundColor Green
    Write-Host "The TabSwitch extension should now appear in your Windows Command Palette." -ForegroundColor Green
}

function Uninstall-Extension {
    Write-Host "Uninstalling TabSwitch Extension..." -ForegroundColor Red
    
    # Remove MSIX package
    $package = Get-AppxPackage | Where-Object { $_.Name -like "*TabSwitch*" }
    if ($package) {
        Remove-AppxPackage -Package $package.PackageFullName
        Write-Host "Command Palette Extension removed." -ForegroundColor Yellow
    }
    
    # Remove native host files
    Remove-Item -Path "$chromeNativeHostDir\com.tabswitch.nativehost.json" -ErrorAction SilentlyContinue
    Remove-Item -Path "$firefoxNativeHostDir\com.tabswitch.nativehost.json" -ErrorAction SilentlyContinue
    Remove-Item -Path "$env:USERPROFILE\AppData\Local\TabSwitch" -Recurse -ErrorAction SilentlyContinue
    Write-Host "Uninstallation completed." -ForegroundColor Green
    Write-Host "Please manually remove browser extensions from chrome://extensions/ and about:debugging" -ForegroundColor Yellow
}

# Main execution
if ($Uninstall) {
    Uninstall-Extension
} else {
    Install-Extension
}
