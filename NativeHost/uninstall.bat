@echo off
setlocal enabledelayedexpansion

echo Uninstalling TabSwitch Native Messaging Host...

REM Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This script must be run as Administrator.
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

REM Remove Chrome native messaging registry entry
echo Removing Chrome native messaging manifest...
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.tabswitch.nativehost" /f 2>nul

REM Remove Firefox native messaging registry entry
echo Removing Firefox native messaging manifest...
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Mozilla\NativeMessagingHosts\com.tabswitch.nativehost" /f 2>nul

REM Remove installation directory
set INSTALL_DIR=C:\Program Files\TabSwitchExtension
if exist "%INSTALL_DIR%" (
    echo Removing installation directory...
    rmdir /S /Q "%INSTALL_DIR%"
)

echo.
echo Uninstallation completed successfully!
echo.
pause
