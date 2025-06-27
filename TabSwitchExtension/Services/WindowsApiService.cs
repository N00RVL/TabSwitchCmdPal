// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TabSwitchExtension.Services;

public class WindowInfo
{
    public IntPtr Handle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public DateTime LastAccessed { get; set; }
    public bool IsVisible { get; set; }
}

public static class WindowsApiService
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool CloseWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    private const int SW_RESTORE = 9;
    private const int SW_SHOW = 5;

    public static List<WindowInfo> GetOpenWindows()
    {
        var windows = new List<WindowInfo>();
        
        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    var builder = new StringBuilder(length + 1);
                    GetWindowText(hWnd, builder, builder.Capacity);
                    
                    string title = builder.ToString();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        GetWindowThreadProcessId(hWnd, out uint processId);
                        
                        try
                        {
                            var process = Process.GetProcessById((int)processId);
                            windows.Add(new WindowInfo
                            {
                                Handle = hWnd,
                                Title = title,
                                ProcessName = process.ProcessName,
                                IsVisible = true,
                                LastAccessed = DateTime.Now
                            });
                        }
                        catch
                        {
                            // Process might have exited, skip it
                        }
                    }
                }
            }
            return true;
        }, IntPtr.Zero);

        return windows;
    }

    public static bool SwitchToWindow(IntPtr windowHandle)
    {
        try
        {
            ShowWindow(windowHandle, SW_RESTORE);
            return SetForegroundWindow(windowHandle);
        }
        catch
        {
            return false;
        }
    }

    public static bool CloseWindowByHandle(IntPtr windowHandle)
    {
        try
        {
            return CloseWindow(windowHandle);
        }
        catch
        {
            return false;
        }
    }

    public static IntPtr GetCurrentForegroundWindow()
    {
        return GetForegroundWindow();
    }
}
