using LionFire.Tmux.PoC.Components;
using LionFire.Tmux.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add REAL Tmux service (requires tmux installed)
builder.Services.AddTmuxService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Check tmux availability on startup
var tmuxService = app.Services.GetRequiredService<LionFire.Tmux.Services.ITmuxService>();
var tmuxAvailable = await tmuxService.IsTmuxAvailableAsync();

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  LionFire.Tmux - Proof of Concept");
Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine($"Tmux Status: {(tmuxAvailable ? "✓ AVAILABLE" : "✗ NOT FOUND")}");
Console.WriteLine($"Service Mode: REAL TMUX SERVICE");
Console.WriteLine();

if (tmuxAvailable)
{
    var sessions = await tmuxService.GetSessionsAsync();
    Console.WriteLine($"Active Sessions: {sessions.Count}");
    foreach (var session in sessions)
    {
        Console.WriteLine($"  • {session.Name} ({session.WindowCount} windows)");
    }
}
else
{
    Console.WriteLine("⚠️  WARNING: Tmux is not installed or not in PATH");
    Console.WriteLine("   The application will run but won't show any sessions.");
    Console.WriteLine();
    Console.WriteLine("   To install tmux:");
    Console.WriteLine("   - Ubuntu/Debian: sudo apt install tmux");
    Console.WriteLine("   - macOS: brew install tmux");
    Console.WriteLine("   - WSL: sudo apt install tmux");
}

Console.WriteLine();
Console.WriteLine("Navigate to: http://localhost:5000");
Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine();

app.Run();
