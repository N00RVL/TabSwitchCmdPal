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
        // Return exactly one command to ensure no duplicates
        return new ICommandItem[]
        {
            new CommandItem(new OpenTabsPage()) 
            { 
                Title = "TabSwitch",
                Subtitle = "🔄 Quick tab switching (Type 'tab' + Enter for best experience)"
            }
        };
    }
}
