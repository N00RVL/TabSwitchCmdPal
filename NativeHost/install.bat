@echo off
setlocal enabledelayedexpansion

echo Installing TabSwitch Native Messaging Host...

REM Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This script must be run as Administrator.
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

REM Create installation directory
set INSTALL_DIR=C:\Program Files\TabSwitchExtension
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

REM Copy native host executable
echo Copying native host executable...
copy /Y "TabSwitchNativeHost.exe" "%INSTALL_DIR%\"
if %errorLevel% neq 0 (
    echo Failed to copy native host executable
    pause
    exit /b 1
)

REM Install Chrome native messaging manifest
echo Installing Chrome native messaging manifest...
set CHROME_REG_KEY=HKEY_LOCAL_MACHINE\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.tabswitch.nativehost
reg add "%CHROME_REG_KEY%" /ve /t REG_SZ /d "%INSTALL_DIR%\chrome_native_manifest.json" /f

REM Copy Chrome manifest
copy /Y "chrome_native_manifest.json" "%INSTALL_DIR%\"

REM Install Firefox native messaging manifest
echo Installing Firefox native messaging manifest...
set FIREFOX_REG_KEY=HKEY_LOCAL_MACHINE\SOFTWARE\Mozilla\NativeMessagingHosts\com.tabswitch.nativehost
reg add "%FIREFOX_REG_KEY%" /ve /t REG_SZ /d "%INSTALL_DIR%\firefox_native_manifest.json" /f

REM Copy Firefox manifest
copy /Y "firefox_native_manifest.json" "%INSTALL_DIR%\"

echo.
echo Installation completed successfully!
echo.
echo Next steps:
echo 1. Install the Chrome extension from: BrowserExtensions\Chrome
echo 2. Install the Firefox extension from: BrowserExtensions\Firefox
echo 3. Configure the TabSwitch Command Palette extension
echo.
pause
