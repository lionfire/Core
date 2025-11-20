using LionFire.Tmux.Models;

namespace LionFire.Tmux.Services;

/// <summary>
/// Mock tmux service for demos and testing (no actual tmux required)
/// </summary>
public class MockTmuxService : ITmuxService
{
    private readonly List<TmuxSession> _sessions;
    private readonly Dictionary<string, List<string>> _windowOutputs;

    public MockTmuxService()
    {
        _sessions = GenerateMockSessions();
        _windowOutputs = GenerateMockOutputs();
    }

    public Task<List<TmuxSession>> GetSessionsAsync()
    {
        // Simulate network delay
        return Task.Delay(100).ContinueWith(_ => _sessions);
    }

    public Task<List<TmuxWindow>> GetWindowsAsync(string sessionName)
    {
        var session = _sessions.FirstOrDefault(s => s.Name == sessionName);
        return Task.FromResult(session?.Windows ?? new List<TmuxWindow>());
    }

    public Task<List<string>> CaptureWindowOutputAsync(string sessionName, int windowIndex, int lines = 100)
    {
        var key = $"{sessionName}:{windowIndex}";
        if (_windowOutputs.TryGetValue(key, out var output))
        {
            return Task.FromResult(output.TakeLast(lines).ToList());
        }
        return Task.FromResult(new List<string> { $"[No output for {key}]" });
    }

    public Task<bool> CreateSessionAsync(string name, string? startCommand = null)
    {
        // Simulate creation
        var newSession = new TmuxSession
        {
            Name = name,
            WindowCount = 1,
            Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Attached = false,
            Activity = DateTime.Now.ToString("HH:mm:ss"),
            CreatedDateTime = DateTime.Now,
            Windows = new List<TmuxWindow>
            {
                new TmuxWindow
                {
                    SessionName = name,
                    Index = 0,
                    Name = "bash",
                    PaneId = "%0",
                    IsActive = true,
                    CurrentCommand = startCommand ?? "bash",
                    LastActivity = DateTime.Now
                }
            }
        };

        _sessions.Add(newSession);
        _windowOutputs[$"{name}:0"] = new List<string>
        {
            $"$ {startCommand ?? "bash"}",
            $"Session '{name}' created",
            "$ "
        };

        return Task.FromResult(true);
    }

    public Task<bool> KillSessionAsync(string name)
    {
        var session = _sessions.FirstOrDefault(s => s.Name == name);
        if (session != null)
        {
            _sessions.Remove(session);
            // Remove associated outputs
            var keysToRemove = _windowOutputs.Keys.Where(k => k.StartsWith($"{name}:")).ToList();
            foreach (var key in keysToRemove)
            {
                _windowOutputs.Remove(key);
            }
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> KillWindowAsync(string sessionName, int windowIndex)
    {
        var session = _sessions.FirstOrDefault(s => s.Name == sessionName);
        if (session != null)
        {
            var window = session.Windows.FirstOrDefault(w => w.Index == windowIndex);
            if (window != null)
            {
                session.Windows.Remove(window);
                session.WindowCount = session.Windows.Count;
                _windowOutputs.Remove($"{sessionName}:{windowIndex}");
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }

    public Task<bool> IsTmuxAvailableAsync()
    {
        // Mock service always reports tmux as available (it's mocking it)
        return Task.FromResult(true);
    }

    public Task<bool> SendKeysAsync(string sessionName, int windowIndex, string keys, bool sendEnter = true)
    {
        // Simulate sending keys by adding the input to the output buffer
        var key = $"{sessionName}:{windowIndex}";
        if (_windowOutputs.ContainsKey(key))
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _windowOutputs[key].Add($"$ {keys}");

            if (sendEnter)
            {
                // Simulate some output from the command
                _windowOutputs[key].Add($"[Mock] Command executed: {keys}");
                _windowOutputs[key].Add($"[Mock] Timestamp: {timestamp}");
            }

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private static List<TmuxSession> GenerateMockSessions()
    {
        var baseTime = DateTime.Now.AddHours(-2);

        return new List<TmuxSession>
        {
            new TmuxSession
            {
                Name = "dev-work",
                WindowCount = 3,
                Created = baseTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Attached = true,
                Activity = DateTime.Now.AddMinutes(-5).ToString("HH:mm:ss"),
                CreatedDateTime = baseTime,
                Windows = new List<TmuxWindow>
                {
                    new TmuxWindow
                    {
                        SessionName = "dev-work",
                        Index = 0,
                        Name = "bash",
                        PaneId = "%0",
                        IsActive = false,
                        CurrentCommand = "bash",
                        LastActivity = DateTime.Now.AddMinutes(-30)
                    },
                    new TmuxWindow
                    {
                        SessionName = "dev-work",
                        Index = 1,
                        Name = "editor",
                        PaneId = "%1",
                        IsActive = true,
                        CurrentCommand = "vim",
                        LastActivity = DateTime.Now.AddMinutes(-2)
                    },
                    new TmuxWindow
                    {
                        SessionName = "dev-work",
                        Index = 2,
                        Name = "server",
                        PaneId = "%2",
                        IsActive = false,
                        CurrentCommand = "dotnet",
                        LastActivity = DateTime.Now.AddMinutes(-1)
                    }
                }
            },
            new TmuxSession
            {
                Name = "axi-jobs",
                WindowCount = 2,
                Created = baseTime.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"),
                Attached = false,
                Activity = DateTime.Now.AddMinutes(-10).ToString("HH:mm:ss"),
                CreatedDateTime = baseTime.AddMinutes(-30),
                Windows = new List<TmuxWindow>
                {
                    new TmuxWindow
                    {
                        SessionName = "axi-jobs",
                        Index = 0,
                        Name = "orchestrator",
                        PaneId = "%3",
                        IsActive = true,
                        CurrentCommand = "axi",
                        LastActivity = DateTime.Now.AddMinutes(-10)
                    },
                    new TmuxWindow
                    {
                        SessionName = "axi-jobs",
                        Index = 1,
                        Name = "monitor",
                        PaneId = "%4",
                        IsActive = false,
                        CurrentCommand = "watch",
                        LastActivity = DateTime.Now.AddSeconds(-30)
                    }
                }
            },
            new TmuxSession
            {
                Name = "experiments",
                WindowCount = 1,
                Created = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                Attached = false,
                Activity = DateTime.Now.AddMinutes(-45).ToString("HH:mm:ss"),
                CreatedDateTime = DateTime.Now.AddHours(-1),
                Windows = new List<TmuxWindow>
                {
                    new TmuxWindow
                    {
                        SessionName = "experiments",
                        Index = 0,
                        Name = "python",
                        PaneId = "%5",
                        IsActive = true,
                        CurrentCommand = "python3",
                        LastActivity = DateTime.Now.AddMinutes(-45)
                    }
                }
            }
        };
    }

    private static Dictionary<string, List<string>> GenerateMockOutputs()
    {
        return new Dictionary<string, List<string>>
        {
            ["dev-work:0"] = new List<string>
            {
                "$ pwd",
                "/home/user/projects/myapp",
                "$ ls -la",
                "total 48",
                "drwxr-xr-x  8 user user 4096 Jan 19 14:30 .",
                "drwxr-xr-x 12 user user 4096 Jan 19 10:15 ..",
                "-rw-r--r--  1 user user  220 Jan 19 10:15 .gitignore",
                "drwxr-xr-x  3 user user 4096 Jan 19 14:20 src",
                "drwxr-xr-x  2 user user 4096 Jan 19 11:30 tests",
                "-rw-r--r--  1 user user 1234 Jan 19 14:30 README.md",
                "$ "
            },
            ["dev-work:1"] = new List<string>
            {
                "~",
                "~",
                "~",
                "\"src/Program.cs\" 45L, 1234B",
                "using System;",
                "using Microsoft.Extensions.DependencyInjection;",
                "",
                "namespace MyApp;",
                "",
                "public class Program",
                "{",
                "    public static async Task Main(string[] args)",
                "    {",
                "        var services = new ServiceCollection();",
                "        // Add services here",
                "        ~",
                "        ~",
                "        ~",
                "[1,1] Top"
            },
            ["dev-work:2"] = new List<string>
            {
                "$ dotnet watch run",
                "\x1B[32minfo\x1B[0m: Microsoft.Hosting.Lifetime[14]",
                "      Now listening on: http://localhost:5000",
                "\x1B[32minfo\x1B[0m: Microsoft.Hosting.Lifetime[0]",
                "      Application started. Press Ctrl+C to shut down.",
                "\x1B[32minfo\x1B[0m: Microsoft.Hosting.Lifetime[0]",
                "      Hosting environment: Development",
                "\x1B[36mwatch\x1B[0m: Started",
                "",
                "\x1B[90m[14:30:15]\x1B[0m \x1B[36minfo\x1B[0m: Request GET /api/status",
                "\x1B[90m[14:30:15]\x1B[0m \x1B[32minfo\x1B[0m: Response 200 OK (45ms)",
                "\x1B[90m[14:30:20]\x1B[0m \x1B[36minfo\x1B[0m: Request POST /api/data",
                "\x1B[90m[14:30:20]\x1B[0m \x1B[32minfo\x1B[0m: Response 201 Created (123ms)"
            },
            ["axi-jobs:0"] = new List<string>
            {
                "$ axi tmux orchestrate",
                "\x1B[1m\x1B[36mAxi Tmux Orchestrator\x1B[0m",
                "\x1B[32m✓\x1B[0m Loaded 3 jobs from queue",
                "\x1B[32m✓\x1B[0m Created session: job-build-001",
                "\x1B[32m✓\x1B[0m Created session: job-test-002",
                "\x1B[32m✓\x1B[0m Created session: job-deploy-003",
                "",
                "\x1B[1mActive Jobs:\x1B[0m",
                "  • job-build-001   \x1B[33m[RUNNING]\x1B[0m  Building project...",
                "  • job-test-002    \x1B[36m[PENDING]\x1B[0m  Waiting for build",
                "  • job-deploy-003  \x1B[36m[PENDING]\x1B[0m  Waiting for tests",
                "",
                "\x1B[90m[14:25:10]\x1B[0m \x1B[32minfo\x1B[0m: Job job-build-001 completed successfully",
                "\x1B[90m[14:25:10]\x1B[0m \x1B[36minfo\x1B[0m: Starting job-test-002",
                "",
                "Monitoring sessions... (Ctrl+C to stop)"
            },
            ["axi-jobs:1"] = new List<string>
            {
                "$ watch -n 2 'axi job list'",
                "Every 2.0s: axi job list",
                "",
                "\x1B[1mAxi Jobs Status\x1B[0m",
                "─────────────────────────────────────────",
                "ID              Status     Progress",
                "─────────────────────────────────────────",
                "job-build-001   \x1B[32mCOMPLETED\x1B[0m  100%",
                "job-test-002    \x1B[33mRUNNING\x1B[0m    67%",
                "job-deploy-003  \x1B[36mPENDING\x1B[0m    0%",
                "─────────────────────────────────────────",
                "Total: 3 | Running: 1 | Completed: 1"
            },
            ["experiments:0"] = new List<string>
            {
                "$ python3",
                "Python 3.11.7 (main, Dec  4 2023, 18:10:11)",
                "[GCC 11.4.0] on linux",
                "Type \"help\", \"copyright\", \"credits\" or \"license\" for more info.",
                ">>> import numpy as np",
                ">>> data = np.array([1, 2, 3, 4, 5])",
                ">>> data.mean()",
                "3.0",
                ">>> data.std()",
                "1.4142135623730951",
                ">>> # Experimenting with data analysis",
                ">>> import pandas as pd",
                ">>> df = pd.DataFrame({'A': [1,2,3], 'B': [4,5,6]})",
                ">>> df",
                "   A  B",
                "0  1  4",
                "1  2  5",
                "2  3  6",
                ">>> "
            }
        };
    }
}
