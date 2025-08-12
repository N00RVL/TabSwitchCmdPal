# 🔄 TabSwitch Extension for PowerToys Command Palette

<div align="center">

<img src="TabSwitchExtension/Assets/logo.png" alt="TabSwitch Logo" width="128" height="128">

**A Command Palette extension for seamless tab switching in Windows**

[![Version](https://img.shields.io/badge/version-0.1.0-blue.svg)](#)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010/11-lightgrey.svg)](#)
[![PowerToys](https://img.shields.io/badge/PowerToys-Command%20Palette-purple.svg)](#)

</div>

---

## ✨ Features

- 🔍 **Instant Tab Search**: Filter open tabs as you type
- ⚡ **Fast Navigation**: Keyboard-driven tab switching
- 🎯 **Smart Matching**: Find tabs by title or application name
- 🔧 **Clean Interface**: Seamlessly integrated with PowerToys Command Palette
- 🛡️ **Robust Error Handling**: Graceful handling of edge cases

## 🚀 Quick Start

### Usage

1. **Open Command Palette** (`Win + Alt + Space` by default)
2. **Type "TabSwitch"** or "tab"
3. **Press Enter** to open the tab list
4. **Type to filter** tabs in real-time
5. **Use arrow keys** to navigate
6. **Press Enter** to switch to selected tab

### Basic Commands

| Action | Keys |
|--------|------|
| Open TabSwitch | Type `TabSwitch` → `Enter` |
| Search tabs | Type to filter instantly |
| Navigate | `↑` `↓` arrow keys |
| Switch to tab | `Enter` |
| Go back | `Escape` or `Alt + ←` |

## 🎯 Current Behavior vs Desired "Files"-like Experience

### Current Implementation ✅
- Type "TabSwitch" → Press Enter → Tab list appears
- Real-time filtering as you type in the tab list
- Arrow key navigation and Enter to switch
- Clean, responsive interface

### Desired "Files"-like Behavior ⏳
- Type "tab" → **Instant activation** (no Enter required)
- Direct search/filtering as you continue typing
- Backspace navigation to return to keyword
- Exactly like built-in "files" command

## 🔬 Technical Insights

### PowerToys Command Palette Extension Limitations

After extensive analysis of the PowerToys source code, we discovered that **built-in commands** like "files" use internal APIs that are **not available to extensions**:

- **DirectCommand interface**: Not exposed to extensions
- **Instant activation**: Reserved for core commands
- **Backspace navigation**: Internal message passing only
- **Extension API constraints**: Security and stability limitations

### Code Quality & Features ✅

Our extension implements:
- **Clean architecture** with proper separation of concerns
- **Async tab enumeration** with robust error handling
- **Real-time filtering** within the extension page
- **Professional UI/UX** with icons and placeholder text
- **Cross-platform builds** (x64, x86, ARM64)
- **Zero build warnings** across all configurations

## 📈 Future Roadmap

### Option 1: Upstream Feature Request ⭐ **Recommended**
We've prepared a comprehensive feature request for the PowerToys team to add DirectCommand support to the Extension API. This would enable true "files"-like behavior for all extensions.

**Status**: [Feature request prepared](FEATURE_REQUEST.md) - Ready for submission

### Option 2: Alternative Solutions
While waiting for upstream changes:
- Enhanced keyword recognition patterns
- Improved user guidance and tooltips
- Performance optimizations
- Additional tab management features

## 🛠️ Development

### Building from Source

```powershell
# Clone and navigate
git clone https://github.com/your-repo/TabSwitchExtension.git
cd TabSwitchExtension

# Restore dependencies
dotnet restore

# Build (choose configuration)
dotnet build --configuration Release
dotnet build --configuration Debug

# Package for deployment
dotnet publish --configuration Release
```

### Project Structure

```
TabSwitchExtension/
├── TabSwitchExtension/           # Main extension project
│   ├── Pages/                    # UI pages (OpenTabsPage)
│   ├── Commands/                 # Command implementations
│   ├── Services/                 # Tab enumeration service
│   ├── Assets/                   # Icons and resources
│   └── *.cs                      # Core extension files
├── NativeHost/                   # Native tab enumeration
├── register-cmdpal-extension.ps1 # Registration script
└── TabSwitchExtension.sln       # Solution file
```

### Key Components

- **`TabSwitchExtensionCommandsProvider`**: Main entry point
- **`OpenTabsPage`**: Primary tab list interface
- **`TabEnumerationService`**: Native tab discovery
- **`DummyCommand`**: Error state handling

## 🤔 FAQ

### Why can't TabSwitch work exactly like the "files" command?

The PowerToys Command Palette has different APIs for built-in commands vs extensions. Built-in commands can activate instantly, but extensions must go through the standard workflow (keyword → Enter → page).

### Will this limitation be fixed?

We're advocating for enhanced Extension API capabilities. The PowerToys team is responsive to well-reasoned feature requests, especially when backed by technical analysis.

### Is the current experience still useful?

Absolutely! While it requires one extra Enter keypress, TabSwitch still provides fast, keyboard-driven tab switching with real-time filtering - a significant productivity improvement.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **PowerToys Team** for the extensible Command Palette architecture
- **Microsoft** for the comprehensive extension documentation
- **Community** for feedback and testing

---

<div align="center">

⭐ **Star this repo if you find it useful!** ⭐

[Report Issues](https://github.com/your-repo/TabSwitchExtension/issues) • [Feature Requests](https://github.com/your-repo/TabSwitchExtension/discussions) • [Contributing Guide](CONTRIBUTING.md)

</div>
