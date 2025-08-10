# PowerToys Command Palette Extension API Enhancement Request

## Summary
Request to add DirectCommand or instant activation capabilities to the Command Palette Extensions SDK to enable "files"-like behavior for custom extensions.

## Current Limitation
Extensions currently require users to press Enter after typing a keyword to activate, unlike built-in commands like "files" which activate instantly and allow direct search/filtering.

## Requested Features

### 1. DirectCommand Interface for Extensions
```csharp
public interface IDirectCommand : ICommand
{
    bool CanHandleDirectActivation(string query);
    IPage GetDirectActivationPage(string query);
}
```

### 2. Instant Activation Support
- Allow extensions to specify keywords that activate immediately (without Enter)
- Enable direct search/filtering as the user types
- Support backspace navigation to go back to keyword

### 3. Enhanced CommandProvider
```csharp
public interface IEnhancedCommandProvider : ICommandProvider
{
    string[] DirectActivationKeywords { get; }
    bool SupportsIncrementalFiltering { get; }
}
```

## Use Case: TabSwitch Extension
This would enable a tab-switching extension that behaves like:
- Type "tab" → instantly shows tab list
- Continue typing → filters tabs in real-time
- Backspace to "ta" → goes back to keyword entry
- No Enter required for activation

## Benefits
- Consistency with built-in command UX
- Better user experience for productivity extensions
- Expanded extension development possibilities

## Implementation Notes
This could be implemented by:
1. Adding new interfaces to the Extensions SDK
2. Updating the Command Palette core to handle direct activation
3. Maintaining backward compatibility with existing extensions

---

**Repository**: https://github.com/microsoft/PowerToys
**Related Issue**: Command Palette extension instant activation
**Extension**: TabSwitchExtension (tab switching for productivity)
