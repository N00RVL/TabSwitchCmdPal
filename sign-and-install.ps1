# TabSwitch Extension MSIX Signing and Installation Script
# Run this script as Administrator after enabling Developer Mode

Write-Host "Signing and Installing TabSwitch Extension..." -ForegroundColor Green

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptRoot

# Check if certificate exists
$cert = Get-ChildItem -Path 'Cert:\CurrentUser\My' | Where-Object { $_.Subject -eq 'CN=TabSwitchExtension' } | Select-Object -First 1

if (-not $cert) {
    Write-Host "Creating self-signed certificate..." -ForegroundColor Yellow
    $cert = New-SelfSignedCertificate -Type Custom -Subject 'CN=TabSwitchExtension' -KeyUsage DigitalSignature -FriendlyName 'TabSwitchExtension' -CertStoreLocation 'Cert:\CurrentUser\My' -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.3', '2.5.29.19={text}')
    Write-Host "Certificate created with thumbprint: $($cert.Thumbprint)" -ForegroundColor Green
}

# Add certificate to Trusted Root Certification Authorities
Write-Host "Adding certificate to Trusted Root..." -ForegroundColor Yellow
try {
    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store([System.Security.Cryptography.X509Certificates.StoreName]::Root, [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser)
    $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    $rootStore.Add($cert)
    $rootStore.Close()
    Write-Host "Certificate added to Trusted Root." -ForegroundColor Green
}
catch {
    Write-Host "Note: You may need to manually add the certificate to Trusted Root Certification Authorities." -ForegroundColor Yellow
}

# Sign the MSIX package using SignTool
$msix = Get-ChildItem -Path "TabSwitchExtension\bin" -Filter "*.msix" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1
if (-not $msix) {
    Write-Host "No MSIX package found under TabSwitchExtension\bin. Build/package the app first." -ForegroundColor Red
    exit 1
}
$msixPath = $msix.FullName

# Find SignTool.exe
$signToolPaths = @(
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22000.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe",
    "${env:ProgramFiles(x86)}\Windows Kits\10\bin\x64\signtool.exe"
)

$signTool = $null
foreach ($path in $signToolPaths) {
    if (Test-Path $path) {
        $signTool = $path
        break
    }
}

if ($signTool) {
    Write-Host "Found SignTool at: $signTool" -ForegroundColor Green
    Write-Host "Signing MSIX package..." -ForegroundColor Yellow
    
    $signResult = & $signTool sign /a /v /fd SHA256 /sha1 $cert.Thumbprint $msixPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "MSIX package signed successfully!" -ForegroundColor Green
        
        # Now install the signed package
        Write-Host "Installing signed MSIX package..." -ForegroundColor Yellow
        try {
            Add-AppxPackage -Path $msixPath
            Write-Host "Command Palette Extension installed successfully!" -ForegroundColor Green
        }
        catch {
            Write-Host "Installation failed: $_" -ForegroundColor Red
            Write-Host "Trying with -AllowUnsigned flag..." -ForegroundColor Yellow
            Add-AppxPackage -Path $msixPath -AllowUnsigned
            Write-Host "Command Palette Extension installed successfully!" -ForegroundColor Green
        }
    }
    else {
        Write-Host "Failed to sign MSIX package. Exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Trying to install unsigned package..." -ForegroundColor Yellow
        Add-AppxPackage -Path $msixPath -AllowUnsigned
    }
}
else {
    Write-Host "SignTool not found. Installing unsigned package..." -ForegroundColor Yellow
    Add-AppxPackage -Path $msixPath -AllowUnsigned
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
