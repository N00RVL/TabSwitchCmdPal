# 🔄 TabSwitch

<div align="center">

<img src="TabSwitchExtension/Assets/logo.png" alt="TabSwitch Logo" width="256" height="256">

**A powerful Command Palette extension for seamless tab switching in Windows**

[![Version](https://img.shields.io/badge/version-0.0.1-blue.svg)](https://github.com/N00RVL/TabSwitchCmdPal)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010/11-lightgrey.svg)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

[🚀 Features](#features) • [📥 Installation](#installation) • [🎯 Usage](#usage) • [🛠️ Development](#development) • [🤝 Contributing](#contributing)

</div>

---

## ✨ Features

TabSwitch enhances your Windows productivity by providing lightning-fast tab switching capabilities through the Command Palette:

- 🔍 **Quick Search**: Instantly find and switch to any open tab
- ⚡ **Lightning Fast**: Optimized for speed and responsiveness
- 🎨 **Beautiful UI**: Clean, modern interface that fits seamlessly with Windows
- 🔧 **Customizable**: Tailored commands for your workflow
- 💡 **Smart Suggestions**: Intelligent tab recommendations
- 🌐 **Universal Support**: Works across multiple applications

## 📥 Installation

### Prerequisites

- Windows 10 (Build 19041) or Windows 11
- .NET 8.0 Runtime
- Command Palette for Windows

### Install from Release

1. Download the latest release from [GitHub Releases](https://github.com/N00RVL/TabSwitchCmdPal/releases)
2. Extract the package
3. Run the installer or sideload the `.appx` package
4. Open Command Palette and enjoy!

### Build from Source

```powershell
# Clone the repository
git clone https://github.com/N00RVL/TabSwitchCmdPal.git
cd TabSwitchCmdPal

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Package the extension
dotnet publish --configuration Release
```

## 🎯 Usage

### Basic Commands

1. **Open Command Palette** (`Win + R` or configured hotkey)
2. **Type "TabSwitch"** to see available commands
3. **Select your desired action** and press Enter

### Available Commands

| Command | Description | Shortcut |
|---------|-------------|----------|
| `TabSwitch` | Open the main TabSwitch interface | - |
| `Switch to Tab` | Quick tab switcher with search | `Ctrl+Tab` |
| `Recent Tabs` | View recently accessed tabs | `Ctrl+Shift+Tab` |
| `Close Tab` | Close specific tabs | `Ctrl+W` |

### Pro Tips

- 💡 **Type partial tab names** for instant filtering
- 🔄 **Use arrow keys** to navigate through results
- ⚡ **Press Enter** to switch immediately
- 🎯 **Use wildcards** (`*`) for advanced searching

### Debugging

```powershell
# Debug build
dotnet build --configuration Debug

# Run with debugger attached
dotnet run --configuration Debug
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- **GitHub Repository**: [TabSwitchCmdPal](https://github.com/N00RVL/TabSwitchCmdPal)
- **Issues & Bug Reports**: [GitHub Issues](https://github.com/N00RVL/TabSwitchCmdPal/issues)
- **Discussions**: [GitHub Discussions](https://github.com/N00RVL/TabSwitchCmdPal/discussions)

---

<div align="center">

⭐ **Star this repo if you find it useful!** ⭐

</div>