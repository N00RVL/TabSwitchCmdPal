// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;
using Windows.Foundation;

namespace TabSwitchExtension;

internal sealed partial class OpenTabsPage : ListPage
{
    public OpenTabsPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "Switch to Tab";
        Name = "OpenTabs";
        PlaceholderText = "🔍 Type to search tabs instantly (use ↑↓ arrows to navigate)...";
    }

    public override IListItem[] GetItems()
    {
        try
        {
            // Get tabs asynchronously - we need to handle this synchronously for now
            var tabsTask = NativeHostService.GetAllTabsAsync();
            tabsTask.Wait();
            var tabs = tabsTask.Result;
            
            return tabs
                .Where(t => t.Type == "tab" && !string.IsNullOrWhiteSpace(t.Title))
                .OrderBy(t => t.IsActive ? 0 : 1) // Active tabs first
                .ThenBy(t => t.Title)
                .Select(tab => new ListItem(new OpenTabCommand(tab))
                {
                    Title = tab.Title,
                    Subtitle = GetTabSubtitle(tab),
                    // Icon = GetBrowserIcon(tab.Browser) // Temporarily disabled
                })
                .ToArray();
        }
        catch
        {
            // Fallback to empty array if native host is not available
            return [
                new ListItem(new DummyCommand()) 
                { 
                    Title = "No browser tabs found", 
                    Subtitle = "Make sure browser extension is installed" 
                }
            ];
        }
    }

    // Custom search method to be called if needed
    public IListItem[] SearchItems(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetItems();
        }

        try
        {
            var tabsTask = NativeHostService.GetAllTabsAsync();
            tabsTask.Wait();
            var tabs = tabsTask.Result;
            
            var filteredTabs = tabs
                .Where(t => t.Type == "tab" && !string.IsNullOrWhiteSpace(t.Title))
                .Where(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                           t.Url.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.IsActive ? 0 : 1) // Active tabs first
                .ThenBy(t => t.Title)
                .Select(tab => new ListItem(new OpenTabCommand(tab))
                {
                    Title = tab.Title,
                    Subtitle = GetTabSubtitle(tab),
                })
                .ToArray();

            return filteredTabs.Length > 0 ? filteredTabs : [
                new ListItem(new DummyCommand()) 
                { 
                    Title = $"No tabs matching '{query}'", 
                    Subtitle = "Try a different search term" 
                }
            ];
        }
        catch
        {
            return [
                new ListItem(new DummyCommand()) 
                { 
                    Title = "Search failed", 
                    Subtitle = "Make sure browser extension is installed" 
                }
            ];
        }
    }

    private static string GetTabSubtitle(TabInfo tab)
    {
        var subtitle = tab.Browser;
        if (!string.IsNullOrEmpty(tab.Url))
        {
            try
            {
                var uri = new System.Uri(tab.Url);
                subtitle += $" • {uri.Host}";
            }
            catch
            {
                subtitle += $" • {tab.Url}";
            }
        }
        return subtitle;
    }

    // Temporarily disabled due to version conflicts
    // private static IconInfo? GetBrowserIcon(string browser)
    // {
    //     return browser.ToLower() switch
    //     {
    //         "chrome" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
    //         "firefox" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
    //         "edge" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
    //         "safari" => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Globe),
    //         _ => IconHelpers.FromSegoeFluentIcon(SegoeFluentIcon.Web)
    //     };
    // }
}

internal sealed partial class OpenTabCommand : ICommand, INotifyPropChanged
{
    private readonly TabInfo _tab;

    public OpenTabCommand(TabInfo tab)
    {
        _tab = tab;
    }

    public string Id => $"tab_{_tab.Url}";
    public string Name => _tab.Title;
    public IIconInfo? Icon => GetBrowserIcon(_tab.Browser);

    // WinRT event for prop changed
#pragma warning disable CS0067 // Event is never used
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
#pragma warning restore CS0067

    public CommandResult Execute()
    {
        try
        {
            var activateTask = NativeHostService.ActivateTabAsync(_tab.Url, _tab.Browser);
            activateTask.Wait();
            var success = activateTask.Result;
            
            if (success)
            {
                return new CommandResult();
            }
            
            return new CommandResult();
        }
        catch (System.Exception)
        {
            return new CommandResult();
        }
    }

    private static IIconInfo? GetBrowserIcon(string browser)
    {
        return browser?.ToLower(CultureInfo.InvariantCulture) switch
        {
            "chrome" => IconHelpers.FromRelativePath("Assets\\chrome.png"),
            "edge" => IconHelpers.FromRelativePath("Assets\\edge.png"),
            "firefox" => IconHelpers.FromRelativePath("Assets\\firefox.png"),
            _ => null
        };
    }
}

internal sealed partial class DummyCommand : ICommand, INotifyPropChanged
{
    public string Id => "dummy";
    public string Name => "Dummy";
    public IIconInfo? Icon => null;

    // WinRT event for prop changed
#pragma warning disable CS0067 // Event is never used
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
#pragma warning restore CS0067

    public CommandResult Execute()
    {
        return new CommandResult();
    }
}
