@echo off
setlocal enabledelayedexpansion

echo Building TabSwitch Complete Solution...
echo =======================================

REM Set build configuration
set BUILD_CONFIG=Release
set TARGET_PLATFORM=x64

REM Create output directory
if not exist "Build" mkdir "Build"

echo.
echo [1/3] Building Command Palette Extension...
cd TabSwitchExtension
dotnet restore
if %errorLevel% neq 0 (
    echo ERROR: Failed to restore Command Palette Extension
    exit /b 1
)

dotnet build --configuration %BUILD_CONFIG%
if %errorLevel% neq 0 (
    echo ERROR: Failed to build Command Palette Extension
    exit /b 1
)

dotnet publish --configuration %BUILD_CONFIG%
if %errorLevel% neq 0 (
    echo ERROR: Failed to publish Command Palette Extension
    exit /b 1
)

echo ✓ Command Palette Extension built successfully

cd..

echo.
echo [2/3] Building Native Messaging Host...
cd NativeHost
dotnet restore
if %errorLevel% neq 0 (
    echo ERROR: Failed to restore Native Host
    exit /b 1
)

dotnet build --configuration %BUILD_CONFIG%
if %errorLevel% neq 0 (
    echo ERROR: Failed to build Native Host
    exit /b 1
)

dotnet publish -c %BUILD_CONFIG% -r win-%TARGET_PLATFORM% --self-contained
if %errorLevel% neq 0 (
    echo ERROR: Failed to publish Native Host
    exit /b 1
)

echo ✓ Native Messaging Host built successfully

cd..

echo.
echo [3/3] Copying Browser Extensions...
xcopy /E /I "BrowserExtensions" "Build\BrowserExtensions"
if %errorLevel% neq 0 (
    echo ERROR: Failed to copy Browser Extensions
    exit /b 1
)

echo ✓ Browser Extensions copied successfully

echo.
echo Copying Native Host files to Build directory...
xcopy /E /I "NativeHost\bin\%BUILD_CONFIG%\net8.0-windows\win-%TARGET_PLATFORM%\publish" "Build\NativeHost"
xcopy "NativeHost\*.bat" "Build\NativeHost\"
xcopy "NativeHost\*_native_manifest.json" "Build\NativeHost\"

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
