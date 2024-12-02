using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Spectre.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var uri = new Uri("ws://localhost:3100/loki/api/v1/tail?query={transit=%22direct%22}");
        var cts = new CancellationTokenSource();

        using (var ws = new ClientWebSocket())
        {
            try
            {
                await ws.ConnectAsync(uri, cts.Token);
                Console.WriteLine("WebSocket connection established.");

                while (ws.State == WebSocketState.Open)
                {
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            var tempBuffer = new byte[4096];
                            var result = await ws.ReceiveAsync(new ArraySegment<byte>(tempBuffer), cts.Token);
                            ms.Write(tempBuffer, 0, result.Count);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                // Handle close message
                                Console.WriteLine("Received Close message from server.");
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
                                break;
                            }

                            // Check if we have a complete JSON message by trying to parse it
                            ms.Position = 0;
                            try
                            {
                                using (var reader = new StreamReader(ms, Encoding.UTF8, leaveOpen: true))
                                {
                                    var jsonString = reader.ReadToEnd();
                                    if (!string.IsNullOrWhiteSpace(jsonString))
                                    {
                                        DisplayColoredLog(jsonString);
                                        break; // Break the loop if we have a complete JSON message
                                    }
                                }
                            }
                            catch (JsonException)
                            {
                                // If JSON parsing fails, continue accumulating data
                                ms.Position = ms.Length; // Reset stream position for appending more data
                            }

                        } while (ws.State == WebSocketState.Open);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseSent)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
                }
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void DisplayColoredLog(string jsonLog)
    {
        Console.WriteLine(jsonLog);

        var log = JsonDocument.Parse(jsonLog);
        var streams = log.RootElement.GetProperty("streams").EnumerateArray();

        int streamCount = 0;
        foreach (var stream in streams)
        {
            streamCount++;
            var values = stream.GetProperty("values").EnumerateArray();
            foreach (var value in values)
            {
                var messageJson = value[1].GetString();
                Console.WriteLine(messageJson);
                var logEntry = JsonDocument.Parse(messageJson).RootElement;
                if(logEntry.TryGetProperty("ContextId", out var ContextId))
                {
                    Console.WriteLine("ContextId: " + ContextId.GetInt32());
                }
                var message = logEntry.GetProperty("Message").GetString();
                var level = logEntry.GetProperty("level").GetString();

                var table = new Table();
                table.AddColumn(new TableColumn("Timestamp").Centered());
                table.AddColumn(new TableColumn("Message").LeftAligned());

                // Parse timestamp to readable format
                if (long.TryParse(value[0].GetString(), out var timestamp))
                {
                    DateTime date;
                    var milliseconds = timestamp / 1_000_000;

                    try
                    {
                         date = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).LocalDateTime;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid timestamp: " + timestamp);
                        // If timestamp is invalid, use current time
                        date = DateTime.Now;
                    }
                    var escapedMessage = message.Replace("[", "[[").Replace("]", "]]");
                    table.AddRow(
                        $"[grey]{date.ToString("yyyy-MM-dd HH:mm:ss")}[/]",
                        ColorMessageByLevel(escapedMessage, level)
                    );
                }

                AnsiConsole.Render(table);
            }
        }
        if(streamCount > 1)
        {
            Console.WriteLine("streamCount: " + streamCount);
        }
    }

    static string ColorMessageByLevel(string message, string level)
    {
        return level switch
        {
            "info" => $"[green]{message}[/]",
            "warn" => $"[yellow]{message}[/]",
            "error" => $"[red]{message}[/]",
            "debug" => $"[blue]{message}[/]",
            _ => $"[blue]{message}[/]"
            //_ => message
        };
    }
}

