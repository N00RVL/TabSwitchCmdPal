// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;

namespace TabSwitchExtension;

internal sealed partial class CloseTabPage : ListPage
{
    public CloseTabPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "Close Tab";
        Name = "CloseTabs";
        IsSearchable = true;
        SearchPlaceholderText = "Search windows to close...";
    }

    public override IListItem[] GetItems()
    {
        var windows = WindowsApiService.GetOpenWindows();
        var currentWindow = WindowsApiService.GetCurrentForegroundWindow();
        
        return windows
            .Where(w => !string.IsNullOrWhiteSpace(w.Title) && w.Handle != currentWindow)
            .OrderBy(w => w.Title)
            .Select(window => new ListItem(new CloseWindowCommand(window))
            {
                Title = $"❌ {window.Title}",
                Subtitle = $"Close {window.ProcessName}",
                Icon = GetProcessIcon(window.ProcessName)
            })
            .ToArray();
    }

    private static Icon? GetProcessIcon(string processName)
    {
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

internal class CloseWindowCommand : ICommand
{
    private readonly WindowInfo _window;

    public CloseWindowCommand(WindowInfo window)
    {
        _window = window;
    }

    public CommandResult Execute()
    {
        var success = WindowsApiService.CloseWindowByHandle(_window.Handle);
        
        if (success)
        {
            return CommandResult.CreateSuccessfulResult();
        }
        
        return CommandResult.CreateErrorResult("Failed to close window");
    }
}
