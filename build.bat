@echo off
setlocal enabledelayedexpansion

pushd "%~dp0"

echo Building TabSwitch Complete Solution...
echo =======================================

REM Set build configuration
set BUILD_CONFIG=Release
set TARGET_PLATFORM=x64
set "DOTNET_CMD=dotnet"

call :ensure_dotnet
if %errorLevel% neq 0 (
    echo ERROR: .NET SDK is required to build this project.
    popd
    exit /b 1
)

REM Create output directory
if not exist "Build" mkdir "Build"
if not exist "Build\NativeHost" mkdir "Build\NativeHost"

echo.
echo [1/3] Building Command Palette Extension...
cd TabSwitchExtension
"%DOTNET_CMD%" restore
if %errorLevel% neq 0 (
    echo ERROR: Failed to restore Command Palette Extension
    popd
    exit /b 1
)

"%DOTNET_CMD%" build --configuration %BUILD_CONFIG% -p:Platform=%TARGET_PLATFORM%
if %errorLevel% neq 0 (
    echo ERROR: Failed to build Command Palette Extension
    popd
    exit /b 1
)

"%DOTNET_CMD%" publish --configuration %BUILD_CONFIG% -p:Platform=%TARGET_PLATFORM%
if %errorLevel% neq 0 (
    echo ERROR: Failed to publish Command Palette Extension
    popd
    exit /b 1
)

echo ✓ Command Palette Extension built successfully

cd ..

echo.
echo [2/3] Building Native Messaging Host...
cd NativeHost
"%DOTNET_CMD%" restore
if %errorLevel% neq 0 (
    echo ERROR: Failed to restore Native Host
    popd
    exit /b 1
)

"%DOTNET_CMD%" build --configuration %BUILD_CONFIG%
if %errorLevel% neq 0 (
    echo ERROR: Failed to build Native Host
    popd
    exit /b 1
)

"%DOTNET_CMD%" publish -c %BUILD_CONFIG% -r win-%TARGET_PLATFORM% --self-contained true -o "..\Build\NativeHost"
if %errorLevel% neq 0 (
    echo ERROR: Failed to publish Native Host
    popd
    exit /b 1
)

echo ✓ Native Messaging Host built successfully

cd ..

echo.
echo [3/3] Copying Browser Extensions...
xcopy /E /I /Y "BrowserExtensions" "Build\BrowserExtensions"
if %errorLevel% neq 0 (
    echo ERROR: Failed to copy Browser Extensions
    popd
    exit /b 1
)

echo ✓ Browser Extensions copied successfully

echo.
echo Copying Native Host files to Build directory...
xcopy /Y "NativeHost\*.bat" "Build\NativeHost\"
xcopy /Y "NativeHost\*_native_manifest.json" "Build\NativeHost\"

echo.
echo =======================================
echo ✓ BUILD COMPLETE
echo =======================================
echo.
echo Output directory: Build\
echo.
echo Next steps:
echo 1. Install Command Palette Extension from TabSwitchExtension\bin\%BUILD_CONFIG%\
echo 2. Run Build\NativeHost\install.bat as Administrator
echo 3. Install browser extensions from Build\BrowserExtensions\
echo.
echo For more information, see README.md
echo.
pause

popd
exit /b 0

:ensure_dotnet
where dotnet >nul 2>&1
if %errorLevel% equ 0 (
    for /f %%S in ('dotnet --list-sdks 2^>nul') do (
        set "HAS_SDK=1"
        goto :sdk_check_done
    )
)

:sdk_check_done
if defined HAS_SDK (
    exit /b 0
)

echo .NET SDK was not found in PATH.
echo Attempting local SDK bootstrap to .dotnet\ ...
set "LOCAL_DOTNET=%CD%\.dotnet"
if exist "%LOCAL_DOTNET%\dotnet.exe" (
    set "DOTNET_CMD=%LOCAL_DOTNET%\dotnet.exe"
    set "PATH=%LOCAL_DOTNET%;%PATH%"
    echo Using local SDK: %DOTNET_CMD%
    exit /b 0
)

powershell -NoProfile -ExecutionPolicy Bypass -File "TabSwitchExtension\dotnet-install.ps1" -Channel 9.0 -InstallDir "%LOCAL_DOTNET%"
if %errorLevel% neq 0 (
    echo ERROR: Failed to install local .NET SDK.
    echo Install .NET SDK 9.0 manually: https://dotnet.microsoft.com/download/dotnet/9.0
    exit /b 1
)

if not exist "%LOCAL_DOTNET%\dotnet.exe" (
    echo ERROR: Local SDK install completed but dotnet.exe was not found.
    exit /b 1
)

set "DOTNET_CMD=%LOCAL_DOTNET%\dotnet.exe"
set "PATH=%LOCAL_DOTNET%;%PATH%"
echo Local SDK ready: %DOTNET_CMD%
exit /b 0
