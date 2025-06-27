// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TabSwitchExtension;

public partial class TabSwitchExtensionCommandsProvider : CommandProvider
{
    public TabSwitchExtensionCommandsProvider()
    {
        DisplayName = "TabSwitch";
        Icon = IconHelpers.FromRelativePath("Assets\\logo.png");
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return [
            new CommandItem(new TabSwitchMainPage()) { Title = "TabSwitch" },
            new CommandItem(new OpenTabsPage()) { Title = "Switch to Tab" },
            new CommandItem(new RecentTabsPage()) { Title = "Recent Tabs" },
            new CommandItem(new CloseTabPage()) { Title = "Close Tab" },
        ];
    }
}
