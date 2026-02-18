// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Diagnostics;
using Windows.Foundation;

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
            new ListItem(new CommandItem(new OpenTabsPage())) { Title = "🔍 Switch to Tab", Subtitle = "Find and switch to any open browser tab" },
            new ListItem(new CommandItem(new RecentTabsPage())) { Title = "⏰ Recent Tabs & History", Subtitle = "View recently accessed tabs and browsing history" },
            new ListItem(new CommandItem(new CloseTabPage())) { Title = "❌ Close Tab", Subtitle = "Close specific browser tabs" },
            new ListItem(new OpenLinkCommand("https://github.com/N00RVL/TabSwitchCmdPal")) { Title = "🔗 GitHub Repository", Subtitle = "View source code and documentation" },
            new ListItem(new OpenLinkCommand("https://github.com/N00RVL/TabSwitchCmdPal/blob/main/README.md#installation")) { Title = "📖 Setup Guide", Subtitle = "Instructions for installing browser extensions" }
        ];
    }
}

internal sealed class OpenLinkCommand : ICommand, INotifyPropChanged
{
    private readonly string _url;
    public OpenLinkCommand(string url) => _url = url;
    public string Id => _url;
    public string Name => _url;
    public IIconInfo? Icon => null;
#pragma warning disable CS0067
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
#pragma warning restore CS0067
    public CommandResult Execute()
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = _url, UseShellExecute = true });
        }
        catch { }
        return new CommandResult();
    }
}
