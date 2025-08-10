using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace TabSwitchNativeHost
{
    public class Program
    {
        private static readonly Dictionary<string, object> TabData = new();
        private static readonly object DataLock = new();

        public static async Task Main(string[] args)
        {
            try
            {
                // Set up input/output streams for native messaging
                using var stdin = Console.OpenStandardInput();
                using var stdout = Console.OpenStandardOutput();

                // Log to a file for debugging (optional)
                var logPath = Path.Combine(Path.GetTempPath(), "tabswitch_host.log");
                
                await ProcessMessagesAsync(stdin, stdout, logPath);
            }
            catch (Exception ex)
            {
                // Log error and exit
                var logPath = Path.Combine(Path.GetTempPath(), "tabswitch_host_error.log");
                await File.WriteAllTextAsync(logPath, $"Error: {ex.Message}\n{ex.StackTrace}");
                Environment.Exit(1);
            }
        }

        private static async Task ProcessMessagesAsync(Stream stdiln, Stream stdout, string logPath)
        {
            var buffer = new byte[4];

            while (true)
            {
                try
                {
                    // Read message length (4 bytes, little endian)
                    int bytesRead = await stdin.ReadAsync(buffer.AsMemory(0, 4));
                    if (bytesRead != 4)
                        break;

                    int messageLength = BitConverter.ToInt32(buffer, 0);
                    if (messageLength <= 0 || messageLength > 1024 * 1024) // Max 1MB
                        break;

                    // Read message content
                    var messageBuffer = new byte[messageLength];
                    int totalRead = 0;
                    while (totalRead < messageLength)
                    {
                        bytesRead = await stdin.ReadAsync(messageBuffer.AsMemory(totalRead, messageLength - totalRead));
                        if (bytesRead == 0)
                            break;
                        totalRead += bytesRead;
                    }

                    if (totalRead != messageLength)
                        break;

                    // Parse and process message
                    string messageJson = Encoding.UTF8.GetString(messageBuffer);
                    await LogMessage(logPath, $"Received: {messageJson}");

                    var message = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(messageJson);
                    var response = await ProcessMessage(message ?? new Dictionary<string, JsonElement>());

                    // Send response
                    if (response != null)
                    {
                        await SendMessage(stdout, response);
                        await LogMessage(logPath, $"Sent: {JsonSerializer.Serialize(response)}");
                    }
                }
                catch (Exception ex)
                {
                    await LogMessage(logPath, $"Error processing message: {ex.Message}");
                    break;
                }
            }
        }

        private static async Task<Dictionary<string, object>> ProcessMessage(Dictionary<string, JsonElement> message)
        {
            try
            {
                if (!message.TryGetValue("action", out var actionElement))
                    return CreateErrorResponse("No action specified");

                string action = actionElement.GetString() ?? "unknown";

                switch (action)
                {
                    case "getAllTabs":
                        return await GetAllTabsResponse();

                    case "updateTabData":
                        if (message.TryGetValue("data", out var dataElement))
                        {
                            await UpdateTabData(dataElement);
                            return CreateSuccessResponse("Tab data updated");
                        }
                        return CreateErrorResponse("No data provided");

                    case "tabActivated":
                        if (message.TryGetValue("data", out var activatedData))
                        {
                            await HandleTabActivated(activatedData);
                            return CreateSuccessResponse("Tab activation recorded");
                        }
                        return CreateErrorResponse("No activation data provided");

                    case "ping":
                        return CreateSuccessResponse("pong");

                    default:
                        return CreateErrorResponse($"Unknown action: {action}");
                }
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"Error processing message: {ex.Message}");
            }
        }

        private static Task<Dictionary<string, object>> GetAllTabsResponse()
        {
            lock (DataLock)
            {
                return Task.FromResult(new Dictionary<string, object>
                {
                    ["success"] = true,
                    ["data"] = new Dictionary<string, object>(TabData),
                    ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
        }

        private static Task UpdateTabData(JsonElement dataElement)
        {
            lock (DataLock)
            {
                // Parse tab data from browser extension
                if (dataElement.TryGetProperty("tabs", out var tabsElement))
                {
                    TabData["currentTabs"] = JsonSerializer.Deserialize<object>(tabsElement.GetRawText()) ?? new object();
                }

                if (dataElement.TryGetProperty("history", out var historyElement))
                {
                    TabData["recentHistory"] = JsonSerializer.Deserialize<object>(historyElement.GetRawText()) ?? new object();
                }

                if (dataElement.TryGetProperty("browser", out var browserElement))
                {
                    TabData["browser"] = browserElement.GetString() ?? "unknown";
                }

                TabData["lastUpdated"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return Task.CompletedTask;
        }

        private static Task HandleTabActivated(JsonElement activatedData)
        {
            lock (DataLock)
            {
                // Store recently activated tab for quick access
                TabData["lastActivatedTab"] = JsonSerializer.Deserialize<object>(activatedData.GetRawText()) ?? new object();
                TabData["lastActivated"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return Task.CompletedTask;
        }

        private static async Task SendMessage(Stream stdout, Dictionary<string, object> message)
        {
            string messageJson = JsonSerializer.Serialize(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);
            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            await stdout.WriteAsync(lengthBytes.AsMemory(0, 4));
            await stdout.WriteAsync(messageBytes.AsMemory(0, messageBytes.Length));
            await stdout.FlushAsync();
        }

        private static Dictionary<string, object> CreateSuccessResponse(string message)
        {
            return new Dictionary<string, object>
            {
                ["success"] = true,
                ["message"] = message,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        private static Dictionary<string, object> CreateErrorResponse(string error)
        {
            return new Dictionary<string, object>
            {
                ["success"] = false,
                ["error"] = error,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        private static async Task LogMessage(string logPath, string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                await File.AppendAllTextAsync(logPath, logEntry);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
