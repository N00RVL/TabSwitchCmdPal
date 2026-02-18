// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TabSwitchExtension.Services;

public class TabInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Favicon { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public DateTime LastAccessed { get; set; }
    public bool IsActive { get; set; }
    public string Type { get; set; } = "tab"; // "tab" or "history"
}

public class NativeHostService
{
    private static readonly string NativeHostPath = @"C:\Program Files\TabSwitchExtension\TabSwitchNativeHost.exe";
    private static Process? _hostProcess;
    private static StreamWriter? _hostInput;
    private static StreamReader? _hostOutput;
    private static readonly object _lockObject = new();

    public static async Task<List<TabInfo>> GetAllTabsAsync()
    {
        try
        {
            await EnsureHostConnectionAsync();
            
            var request = new Dictionary<string, object>
            {
                ["action"] = "getAllTabs"
            };

            var response = await SendMessageAsync(request);
            if (response != null && response.TryGetValue("success", out var successElement) && 
                successElement.GetBoolean() && response.TryGetValue("data", out var dataElement))
            {
                return ParseTabData(dataElement);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting tabs from native host: {ex.Message}");
        }

        return new List<TabInfo>();
    }

    public static async Task<bool> ActivateTabAsync(string url, string browser = "")
    {
        try
        {
            // For now, we'll open the URL in the default browser
            // In the future, this could be enhanced to switch to specific browser tabs
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            // Notify the native host about the activation
            await NotifyTabActivationAsync(url, browser);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error activating tab: {ex.Message}");
            return false;
        }
    }

    private static async Task NotifyTabActivationAsync(string url, string browser)
    {
        try
        {
            await EnsureHostConnectionAsync();
            
            var request = new Dictionary<string, object>
            {
                ["action"] = "tabActivated",
                ["data"] = new Dictionary<string, object>
                {
                    ["url"] = url,
                    ["browser"] = browser,
                    ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                }
            };

            await SendMessageAsync(request);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error notifying tab activation: {ex.Message}");
        }
    }

    private static async Task EnsureHostConnectionAsync()
    {
        lock (_lockObject)
        {
            if (_hostProcess != null && !_hostProcess.HasExited)
                return;

            try
            {
                _hostProcess?.Kill();
                _hostProcess?.Dispose();
            }
            catch { }

            _hostProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = NativeHostPath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            _hostProcess.Start();
            _hostInput = _hostProcess.StandardInput;
            _hostOutput = _hostProcess.StandardOutput;
        }

        // Test connection with ping
        try
        {
            var pingRequest = new Dictionary<string, object> { ["action"] = "ping" };
            var response = await SendMessageAsync(pingRequest);
            if (response == null || !response.TryGetValue("success", out var success) || !success.GetBoolean())
            {
                throw new Exception("Native host ping failed");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Native host connection test failed: {ex.Message}");
            throw;
        }
    }

    private static async Task<Dictionary<string, JsonElement>?> SendMessageAsync(Dictionary<string, object> message)
    {
        if (_hostInput == null || _hostOutput == null)
            return null;

        try
        {
            // Serialize message
            string messageJson = JsonSerializer.Serialize(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);
            
            // Send message length (4 bytes, little endian)
            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);
            await _hostInput.BaseStream.WriteAsync(lengthBytes, 0, 4);
            
            // Send message content
            await _hostInput.BaseStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            await _hostInput.BaseStream.FlushAsync();

            // Read response length
            byte[] responseLengthBytes = new byte[4];
            await _hostOutput.BaseStream.ReadAsync(responseLengthBytes, 0, 4);
            int responseLength = BitConverter.ToInt32(responseLengthBytes, 0);

            if (responseLength <= 0 || responseLength > 1024 * 1024) // Max 1MB
                return null;

            // Read response content
            byte[] responseBytes = new byte[responseLength];
            int totalRead = 0;
            while (totalRead < responseLength)
            {
                int bytesRead = await _hostOutput.BaseStream.ReadAsync(responseBytes, totalRead, responseLength - totalRead);
                if (bytesRead == 0)
                    break;
                totalRead += bytesRead;
            }

            if (totalRead != responseLength)
                return null;

            // Parse response
            string responseJson = Encoding.UTF8.GetString(responseBytes);
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseJson);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error communicating with native host: {ex.Message}");
            return null;
        }
    }

    private static List<TabInfo> ParseTabData(JsonElement dataElement)
    {
        var tabs = new List<TabInfo>();

        try
        {
            // Parse current tabs
            if (dataElement.TryGetProperty("currentTabs", out var currentTabsElement))
            {
                if (currentTabsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var tab in currentTabsElement.EnumerateArray())
                    {
                        tabs.Add(ParseSingleTab(tab, "tab"));
                    }
                }
            }

            // Parse recent history
            if (dataElement.TryGetProperty("recentHistory", out var historyElement))
            {
                if (historyElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var historyItem in historyElement.EnumerateArray())
                    {
                        tabs.Add(ParseSingleTab(historyItem, "history"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error parsing tab data: {ex.Message}");
        }

        return tabs;
    }

    private static TabInfo ParseSingleTab(JsonElement tabElement, string type)
    {
        var tab = new TabInfo { Type = type };

        try
        {
            if (tabElement.TryGetProperty("id", out var idProp))
                tab.Id = idProp.GetString() ?? "";

            if (tabElement.TryGetProperty("title", out var titleProp))
                tab.Title = titleProp.GetString() ?? "";

            if (tabElement.TryGetProperty("url", out var urlProp))
                tab.Url = urlProp.GetString() ?? "";

            if (tabElement.TryGetProperty("favicon", out var faviconProp))
                tab.Favicon = faviconProp.GetString() ?? "";

            if (tabElement.TryGetProperty("browser", out var browserProp))
                tab.Browser = browserProp.GetString() ?? "";

            if (tabElement.TryGetProperty("active", out var activeProp))
                tab.IsActive = activeProp.GetBoolean();

            if (tabElement.TryGetProperty("timestamp", out var timestampProp))
            {
                if (timestampProp.TryGetInt64(out var timestamp))
                {
                    tab.LastAccessed = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error parsing individual tab: {ex.Message}");
        }

        return tab;
    }

    public static void Dispose()
    {
        try
        {
            _hostInput?.Dispose();
            _hostOutput?.Dispose();
            _hostProcess?.Kill();
            _hostProcess?.Dispose();
        }
        catch { }
    }
}
