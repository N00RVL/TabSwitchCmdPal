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
    }

    public override IListItem[] GetItems()
    {
        try
        {
            // Get tabs and history from native host
            var tabsTask = NativeHostService.GetAllTabsAsync();
            tabsTask.Wait();
            var allItems = tabsTask.Result;
            
            // Combine recent tabs and history, prioritizing recent activity
            var recentItems = allItems
                .Where(item => !string.IsNullOrWhiteSpace(item.Title))
                .OrderByDescending(item => item.LastAccessed)
                .Take(20) // Show last 20 items
                .ToList();
            
            if (recentItems.Count == 0)
            {
                return [
                    new ListItem(new NoOpCommand()) 
                    { 
                        Title = "No recent tabs or history", 
                        Subtitle = "Install browser extensions and browse some websites to see them here",
                        Icon = null
                    }
                ];
            }
            
            return recentItems
                .Select(item => new ListItem(new OpenTabCommand(item))
                {
                    Title = item.Title,
                    Subtitle = GetItemSubtitle(item),
                    Icon = null
                })
                .ToArray();
        }
        catch
        {
            // Fallback if native host is not available
            return [
                new ListItem(new NoOpCommand()) 
                { 
                    Title = "Browser extensions not connected", 
                    Subtitle = "Install and configure browser extensions to see tabs and history",
                    Icon = null
                }
            ];
        }
    }

    private static string GetItemSubtitle(TabInfo item)
    {
        var subtitle = $"{item.Browser}";
        
        if (item.Type == "history")
            subtitle += " • History";
        else if (item.IsActive)
            subtitle += " • Active";
            
        subtitle += $" • {GetRelativeTime(item.LastAccessed)}";
        
        if (!string.IsNullOrEmpty(item.Url))
        {
            try
            {
                var uri = new System.Uri(item.Url);
                subtitle += $" • {uri.Host}";
            }
            catch
            {
                // URL parsing failed, skip adding host
            }
        }
        
        return subtitle;
    }

    private static string GetRelativeTime(System.DateTime dateTime)
    {
        var timeSpan = System.DateTime.Now - dateTime;
        
        return timeSpan.TotalMinutes switch
        {
            < 1 => "Just now",
            < 60 => $"{(int)timeSpan.TotalMinutes}m ago",
            < 1440 => $"{(int)timeSpan.TotalHours}h ago",
            _ => $"{(int)timeSpan.TotalDays}d ago"
        };
    }
}
