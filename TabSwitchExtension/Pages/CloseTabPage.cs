// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;
using Windows.Foundation;

namespace TabSwitchExtension;

internal sealed partial class CloseTabPage : ListPage
{
    public CloseTabPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "Close Window";
        Name = "CloseWindows";
        PlaceholderText = "Search windows to close...";
    }

    public override IListItem[] GetItems()
    {
        // For now, show browser windows that can be closed
        // Individual tab closing would require browser extension enhancement
        var windows = WindowsApiService.GetOpenWindows();
        var currentWindow = WindowsApiService.GetCurrentForegroundWindow();
        
        var closeableWindows = windows
            .Where(w => !string.IsNullOrWhiteSpace(w.Title) && w.Handle != currentWindow)
            .OrderBy(w => w.Title)
            .ToList();

        if (closeableWindows.Count == 0)
        {
            return [
                new ListItem(new NoOpCommand()) 
                { 
                    Title = "No windows to close", 
                    Subtitle = "All windows are either the current window or cannot be closed",
                    Icon = null
                }
            ];
        }

        var items = closeableWindows
            .Select(window => new ListItem(new CloseWindowCommand(window))
            {
                Title = $"❌ {window.Title}",
                Subtitle = GetCloseSubtitle(window),
                Icon = null
            })
            .ToList();

        // Add info about browser tab closing
        items.Insert(0, new ListItem(new NoOpCommand()) 
        { 
            Title = "ℹ️ Individual tab closing coming soon", 
            Subtitle = "Currently showing browser windows. Individual tab closing requires browser extension updates.",
            Icon = null
        });

        return items.ToArray();
    }

    private static string GetCloseSubtitle(WindowInfo window)
    {
        var processName = window.ProcessName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
        if (processName.Contains("chrome") || processName.Contains("firefox") || 
            processName.Contains("edge") || processName.Contains("safari"))
        {
            return $"Close {window.ProcessName} browser window (all tabs)";
        }
        return $"Close {window.ProcessName} window";
    }

    private static IIconInfo? GetProcessIcon(string processName)
    {
        // Simplify: no per-process icons for now
        return null;
    }
}

internal sealed class CloseWindowCommand : ICommand, INotifyPropChanged
{
    private readonly WindowInfo _window;

    public CloseWindowCommand(WindowInfo window)
    {
        _window = window;
    }

    public string Id => $"win_{_window.Handle}";
    public string Name => _window.Title;
    public IIconInfo? Icon => null;

    // WinRT event for prop changed (not used)
#pragma warning disable CS0067
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
#pragma warning restore CS0067

    public CommandResult Execute()
    {
        var success = WindowsApiService.CloseWindowByHandle(_window.Handle);
        
        // CommandResult doesn't carry payload in this SDK; success/failure is implicit
        return new CommandResult();
    }
}
