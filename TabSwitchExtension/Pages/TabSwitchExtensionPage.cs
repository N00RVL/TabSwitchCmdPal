// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TabSwitchExtension;

internal sealed partial class TabSwitchMainPage : ListPage
{
    public TabSwitchMainPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
        Title = "TabSwitch";
        Name = "Main";
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new CommandItem(new OpenTabsPage())) { Title = "🔍 Switch to Tab", Subtitle = "Find and switch to any open window" },
            new ListItem(new CommandItem(new RecentTabsPage())) { Title = "⏰ Recent Tabs", Subtitle = "View recently accessed windows" },
            new ListItem(new CommandItem(new CloseTabPage())) { Title = "❌ Close Tab", Subtitle = "Close specific windows" },
            new ListItem(new OpenURLCommand("https://github.com/N00RVL/TabSwitchCmdPal")) { Title = "🔗 GitHub Repository", Subtitle = "View source code and documentation" }
        ];
    }
}
