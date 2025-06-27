// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;

namespace TabSwitchExtension;

internal sealed partial class RecentTabsPage : ListPage
{
    public RecentTabsPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "Recent Tabs";
        Name = "RecentTabs";
        IsSearchable = true;
        SearchPlaceholderText = "Search recent windows...";
    }

    public override IListItem[] GetItems()
    {
        var recentTabs = RecentTabsService.GetRecentTabs();
        
        if (!recentTabs.Any())
        {
            return [
                new ListItem(new NoOpCommand()) 
                { 
                    Title = "No recent tabs", 
                    Subtitle = "Switch to some windows first to see them here",
                    Icon = IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Info)
                }
            ];
        }
        
        return recentTabs
            .Select((window, index) => new ListItem(new SwitchToWindowCommand(window))
            {
                Title = window.Title,
                Subtitle = $"{window.ProcessName} • {GetRelativeTime(window.LastAccessed)}",
                Icon = GetProcessIcon(window.ProcessName)
            })
            .ToArray();
    }

    private static string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        return timeSpan.TotalMinutes switch
        {
            < 1 => "Just now",
            < 60 => $"{(int)timeSpan.TotalMinutes}m ago",
            < 1440 => $"{(int)timeSpan.TotalHours}h ago",
            _ => $"{(int)timeSpan.TotalDays}d ago"
        };
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
