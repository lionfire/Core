using LionFire.Tmux.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Tmux.Extensions;

/// <summary>
/// Extension methods for registering tmux services
/// </summary>
public static class TmuxServiceExtensions
{
    /// <summary>
    /// Add mock tmux service (no actual tmux required - for demos/testing)
    /// </summary>
    public static IServiceCollection AddMockTmuxService(this IServiceCollection services)
    {
        services.AddSingleton<ITmuxService, MockTmuxService>();
        return services;
    }

    /// <summary>
    /// Add real tmux service (requires tmux installed)
    /// Gracefully falls back to mock if tmux is not available
    /// </summary>
    public static IServiceCollection AddTmuxService(this IServiceCollection services)
    {
        services.AddSingleton<ITmuxService, TmuxService>();
        return services;
    }

    /// <summary>
    /// Add tmux service with automatic detection
    /// Uses real tmux if available, falls back to mock if not
    /// </summary>
    public static async Task<IServiceCollection> AddTmuxServiceWithAutoDetectionAsync(
        this IServiceCollection services)
    {
        // Check if tmux is available
        var testService = new TmuxService();
        var tmuxAvailable = await testService.IsTmuxAvailableAsync();

        if (tmuxAvailable)
        {
            Console.WriteLine("✓ Tmux detected - using real tmux service");
            services.AddSingleton<ITmuxService, TmuxService>();
        }
        else
        {
            Console.WriteLine("✗ Tmux not detected - using mock service");
            services.AddSingleton<ITmuxService, MockTmuxService>();
        }

        return services;
    }
}
