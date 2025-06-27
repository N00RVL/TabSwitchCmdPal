// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;

namespace TabSwitchExtension;

internal sealed partial class OpenTabsPage : ListPage
{
    public OpenTabsPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "Switch to Tab";
        Name = "OpenTabs";
        IsSearchable = true;
        SearchPlaceholderText = "Search windows...";
    }

    public override IListItem[] GetItems()
    {
        var windows = WindowsApiService.GetOpenWindows();
        
        return windows
            .Where(w => !string.IsNullOrWhiteSpace(w.Title))
            .OrderBy(w => w.Title)
            .Select(window => new ListItem(new SwitchToWindowCommand(window))
            {
                Title = window.Title,
                Subtitle = $"{window.ProcessName}",
                Icon = GetProcessIcon(window.ProcessName)
            })
            .ToArray();
    }

    private static Icon? GetProcessIcon(string processName)
    {
        // You could enhance this to get actual process icons
        return processName.ToLower() switch
        {
            "chrome" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
            "firefox" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
            "msedge" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
            "notepad" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Document),
            "code" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Code),
            "explorer" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Folder),
            _ => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.App)
        };
    }
}

internal class SwitchToWindowCommand : ICommand
{
    private readonly WindowInfo _window;

    public SwitchToWindowCommand(WindowInfo window)
    {
        _window = window;
    }

    public CommandResult Execute()
    {
        var success = WindowsApiService.SwitchToWindow(_window.Handle);
        
        if (success)
        {
            // Add to recent tabs
            RecentTabsService.AddToRecent(_window);
            return CommandResult.CreateSuccessfulResult();
        }
        
        return CommandResult.CreateErrorResult("Failed to switch to window");
    }
}
