// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TabSwitchExtension.Services;

public class RecentTabsService
{
    private static readonly List<WindowInfo> _recentTabs = new();
    private const int MaxRecentTabs = 20;

    public static void AddToRecent(WindowInfo window)
    {
        // Remove if already exists
        _recentTabs.RemoveAll(w => w.Handle == window.Handle);
        
        // Add to front
        _recentTabs.Insert(0, window);
        
        // Keep only the most recent
        while (_recentTabs.Count > MaxRecentTabs)
        {
            _recentTabs.RemoveAt(_recentTabs.Count - 1);
        }
    }

    public static List<WindowInfo> GetRecentTabs()
    {
        // Filter out closed windows
        var validTabs = new List<WindowInfo>();
        var allWindows = WindowsApiService.GetOpenWindows();
        
        foreach (var recentTab in _recentTabs)
        {
            var stillExists = allWindows.Any(w => w.Handle == recentTab.Handle);
            if (stillExists)
            {
                validTabs.Add(recentTab);
            }
        }
        
        // Update the list to remove closed windows
        _recentTabs.Clear();
        _recentTabs.AddRange(validTabs);
        
        return validTabs;
    }

    public static void ClearRecent()
    {
        _recentTabs.Clear();
    }
}
