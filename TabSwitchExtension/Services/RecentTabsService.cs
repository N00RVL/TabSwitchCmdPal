// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace TabSwitchExtension.Services;

public class RecentTabsService
{
    // Simplified - no recent tracking needed for basic tab opening
    // Keeping this class for compatibility but with minimal functionality
    
    public static void AddToRecent(TabInfo tab)
    {
        // No-op - we're not tracking recent tabs in the simplified version
    }

    public static List<TabInfo> GetRecentTabs()
    {
        // Return empty list - no recent tabs in simplified version
        return new List<TabInfo>();
    }

    public static void ClearRecent()
    {
        // No-op - nothing to clear
    }

    // Overload for backward compatibility with WindowInfo
    public static void AddToRecent(WindowInfo window)
    {
        // No-op - not tracking recent items
    }
}
