// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TabSwitchExtension.Services;
using Windows.Foundation;

namespace TabSwitchExtension;

// Direct command that immediately navigates to tab switching
internal sealed partial class DirectTabSwitchCommand : ICommand, INotifyPropChanged
{
    public string Id => "direct_tab_switch";
    public string Name => "`";
    public IIconInfo? Icon => IconHelpers.FromRelativePath("Assets\\logo.png");

    // WinRT event for prop changed
#pragma warning disable CS0067 // Event is never used
    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;
#pragma warning restore CS0067

    public CommandResult Execute()
    {
        // For now, return a simple result - the navigation will be handled differently
        return new CommandResult();
    }
}
