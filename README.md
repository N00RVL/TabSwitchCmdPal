# 🔄 TabSwitch - BrowTabSwitch provides a simple way to find and open browser tabs across all your browsers through the Windows Command Palette:

- 🌐 **Cross-Browser Tab Access**: List tabs from Chrome, Firefox, Edge
- 🔍 **Smart Search**: Quickly find tabs by title or URL
- ⚡ **One-Click Open**: Select a tab and it opens in the browser
- 🛡️ **Privacy-First**: All data stays local, no cloud sync
- 📱 **Simple Interface**: Clean, straightforward designanagement for Windows Command Palette

<div align="center">

<img src="TabSwitchExtension/Assets/logo.png" alt="TabSwitch Logo" width="256" height="256">

**A simple Command Palette extension for opening browser tabs in Windows**

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/N00RVL/TabSwitchCmdPal)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010/11-lightgrey.svg)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

[🚀 Features](#-features) • [📥 Installation](#-installation) • [🎯 Usage](#-usage) • [🏗️ Architecture](#-architecture) • [🛠️ Development](#-development)

</div>

---

## ✨ Features

TabSwitch revolutionizes browser tab management by providing unified access to tabs and history across all your browsers through the Windows Command Palette:

- 🌐 **Universal Tab Access**: List and switch to tabs from Chrome, Firefox, Edge, and other browsers
- 📚 **Browsing History**: Search through your recent browsing history across all browsers
- 🔍 **Smart Search**: Quickly find tabs and history by title, URL, or domain
- ⏰ **Recent Activity**: View recently accessed tabs and websites
- 🔄 **Cross-Browser**: Works with multiple browsers simultaneously
- ⚡ **Lightning Fast**: Optimized native messaging for instant results
- �️ **Privacy-First**: All data stays local, no cloud sync required

### Components

1. **Browser Extensions** - Installed in each browser to access tab and history data
2. **Native Messaging Host** - Secure bridge between browser extensions and Windows
3. **Command Palette Extension** - Windows application providing the unified interface

## 📥 Installation

### Prerequisites

- Windows 10 (Build 19041) or Windows 11
- Command Palette for Windows
- Supported browsers: Chrome, Firefox, Edge, or Safari
- Administrator access (for native host installation)

### Step 1: Install the Command Palette Extension

1. Download the latest MSIX package from [GitHub Releases](https://github.com/N00RVL/TabSwitchCmdPal/releases)
2. Install the package (may require enabling sideloading)
3. The extension will appear in your Command Palette

### Step 2: Install the Native Messaging Host

```powershell
# Open PowerShell as Administrator
cd NativeHost
.\install.bat
```

This installs the secure bridge between browsers and Windows.

### Step 3: Install Browser Extensions

#### Chrome/Edge

1. Navigate to `chrome://extensions/` (or `edge://extensions/`)
2. Enable "Developer mode"
3. Click "Load unpacked"
4. Select the `BrowserExtensions\Chrome` folder
5. Note the extension ID for configuration

#### Firefox

1. Navigate to `about:debugging#/runtime/this-firefox`
2. Click "Load Temporary Add-on"
3. Select `manifest.json` in `BrowserExtensions\Firefox`

### Step 4: Configuration

Update extension IDs in native host manifests:
- `NativeHost\chrome_native_manifest.json`
- `NativeHost\firefox_native_manifest.json`

Restart browsers to complete setup.

## 🎯 Usage

### Simple Tab Opening

1. **Open Command Palette** (`Win + R` or your configured hotkey)
2. **Type "Open Tab"** or just "tab"
3. **Search** by page title or URL
4. **Press Enter** to open the selected tab in its browser

### Search Tips

- 🎯 **By Title**: `"Gmail"`, `"GitHub"`, `"Documentation"`
- 🌐 **By URL**: `"localhost:3000"`, `"admin.example.com"`
- ⚡ **Instant Results**: Start typing and see tabs filtered immediately

## 🔧 Configuration

### Browser Extension Settings

Access via extension popup in each browser:

- **Tab Collection**: Enable/disable automatic enumeration
- **History Depth**: Control how much history is accessible
- **Update Frequency**: Set sync interval
- **Privacy Settings**: Exclude private/incognito tabs

### Native Host Configuration

Edit configuration files as needed:

- **Logging**: Control debug output level
- **Memory**: Set cache limits
- **Security**: Configure browser permissions

## 🛠️ Development

### Building from Source

```powershell
# Clone repository
git clone https://github.com/N00RVL/TabSwitchCmdPal.git
cd TabSwitchCmdPal

# Build Command Palette Extension
cd TabSwitchExtension
dotnet restore
dotnet build --configuration Release
dotnet publish

# Build Native Host
cd ../NativeHost
dotnet build --configuration Release
dotnet publish -c Release -r win-x64 --self-contained

# Browser extensions require no build step
```

### APIs and Technologies

- **Command Palette**: Microsoft.CommandPalette.Extensions
- **Native Messaging**: Chrome/Firefox native messaging protocols
- **Browser APIs**: `chrome.tabs`, `chrome.history`, `browser.tabs`, `browser.history`
- **.NET 8.0**: Modern C# with native Windows integration

## 🔒 Privacy & Security

TabSwitch is designed with privacy as a core principle:

- 🏠 **Local Only**: All data remains on your machine
- 🚫 **No Cloud Sync**: No external data transmission
- 🔐 **Secure Channels**: Encrypted native messaging
- 🎭 **Permission-Based**: Only accesses explicitly granted data
- 👤 **Private Browsing**: Incognito/private data excluded by default

## 🐛 Troubleshooting

### Common Issues

**Extension Not Responding**
- Verify native host installation: Check Windows Registry
- Ensure browser extensions are enabled
- Restart browsers after installation

**No Tabs Visible**
- Check browser extension permissions
- Review native host logs in `%TEMP%\tabswitch_host.log`
- Verify extension IDs match manifests

**Performance Issues**
- Reduce history range in extension settings
- Clear cached data using Command Palette
- Update to latest version

### Debug Information

```powershell
# Check native host installation
reg query "HKLM\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.tabswitch.nativehost"

# View logs
type "%TEMP%\tabswitch_host.log"

# Test native host directly
cd NativeHost
.\TabSwitchNativeHost.exe
```

### Priority Areas

- 🌍 Additional browser support (Safari, Opera, Brave)
- 🎨 Enhanced UI/UX improvements
- ⚡ Performance optimizations
- 🔧 Additional tab management features
- ♿ Accessibility enhancements

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<div align="center">

⭐ **Star this repo if you find it useful!** ⭐

**Note**: Full functionality requires browser extension installation. Without extensions, TabSwitch provides basic window enumeration as a fallback.

</div>