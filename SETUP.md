# TabSwitch Setup Guide

This guide will walk you through setting up TabSwitch with all its components.

## Quick Start

1. **Run the build script** (optional, if building from source):
   ```cmd
   build.bat
   ```

2. **Install the Native Host** (run as Administrator):
   ```cmd
   cd NativeHost
   install.bat
   ```

3. **Install browser extensions** in your browsers

4. **Use TabSwitch** from the Command Palette!

## Detailed Setup Instructions

### Prerequisites

- Windows 10 (19041+) or Windows 11
- Command Palette for Windows
- Chrome, Firefox, or Edge browser
- Administrator access (for native host)

### Step 1: Build the Solution (if from source)

If you downloaded the source code:

```cmd
# From the project root directory
build.bat
```

This will:
- Build the Command Palette Extension
- Build the Native Messaging Host
- Copy browser extensions to the output directory

### Step 2: Install Command Palette Extension

#### Option A: From Release Package
1. Download the MSIX package from GitHub Releases
2. Double-click to install
3. Enable developer mode if prompted

#### Option B: From Source Build
1. Navigate to `TabSwitchExtension\bin\Release\`
2. Install the generated MSIX package
3. Or use Visual Studio to deploy directly

### Step 3: Install Native Messaging Host

**Important: This must be run as Administrator**

```cmd
cd NativeHost
install.bat
```

This will:
- Copy the native host executable to Program Files
- Register native messaging manifests for Chrome and Firefox
- Set up Windows registry entries

To verify installation:
```cmd
reg query "HKLM\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.tabswitch.nativehost"
reg query "HKLM\SOFTWARE\Mozilla\NativeMessagingHosts\com.tabswitch.nativehost"
```

### Step 4: Install Browser Extensions

#### Chrome/Edge

1. Open Chrome and go to `chrome://extensions/`
2. Enable "Developer mode" (toggle in top right)
3. Click "Load unpacked"
4. Select the `BrowserExtensions\Chrome` folder
5. **Important**: Note the Extension ID that appears
6. Update `NativeHost\chrome_native_manifest.json` with this ID:
   ```json
   {
     "name": "com.tabswitch.nativehost",
     "description": "TabSwitch Native Messaging Host",
     "path": "C:\\Program Files\\TabSwitchExtension\\TabSwitchNativeHost.exe",
     "type": "stdio",
     "allowed_origins": [
       "chrome-extension://YOUR-EXTENSION-ID-HERE/"
     ]
   }
   ```
7. Re-run `install.bat` to update the manifest

#### Firefox

1. Open Firefox and go to `about:debugging#/runtime/this-firefox`
2. Click "Load Temporary Add-on"
3. Navigate to `BrowserExtensions\Firefox\`
4. Select `manifest.json`
5. **Note**: This is temporary and will be removed when Firefox restarts
6. For permanent installation, you'll need to package and sign the extension

### Step 5: Verification

1. **Test Native Host**:
   ```cmd
   cd NativeHost
   echo {"action":"ping"} | TabSwitchNativeHost.exe
   ```

2. **Test Browser Extension**:
   - Click the TabSwitch extension icon in your browser
   - Check that tabs are listed
   - Verify communication with native host

3. **Test Command Palette**:
   - Open Command Palette (Win + R)
   - Type "TabSwitch"
   - Select "Switch to Tab"
   - Verify tabs from browsers appear

## Troubleshooting

### Extension Not Loading
- Check that you're running as Administrator for installation
- Verify Windows version compatibility
- Check Command Palette logs

### Native Host Issues
- Ensure manifests have correct extension IDs
- Check Windows Registry entries
- Review logs in `%TEMP%\tabswitch_host.log`

### Browser Extension Issues
- Verify permissions are granted
- Check browser console for errors
- Ensure extension is enabled
- For Firefox, reload temporary extension after restart

### No Tabs Appearing
- Check that browser extensions are communicating with native host
- Verify native host is receiving data
- Test with a simple webpage open

## Development Setup

If you're developing or modifying TabSwitch:

1. **Set up debugging**:
   ```cmd
   # Debug builds
   dotnet build --configuration Debug
   ```

2. **Enable native host logging**:
   - Native host logs to `%TEMP%\tabswitch_host.log`
   - Browser extensions log to browser console

3. **Test individual components**:
   - Test Command Palette extension standalone
   - Test native host with manual input
   - Test browser extensions independently

## Security Notes

- Native messaging uses secure, local-only communication
- No data is sent over the internet
- Browser extensions only access tabs/history you grant permission for
- All communication stays within your local machine

## Uninstallation

To completely remove TabSwitch:

1. **Uninstall Command Palette Extension**:
   - Through Windows Settings > Apps
   - Or use PowerShell: `Remove-AppxPackage`

2. **Remove Native Host**:
   ```cmd
   cd NativeHost
   uninstall.bat
   ```

3. **Remove Browser Extensions**:
   - Chrome: Go to `chrome://extensions/` and remove
   - Firefox: Go to `about:addons` and remove

This will completely remove all TabSwitch components from your system.
