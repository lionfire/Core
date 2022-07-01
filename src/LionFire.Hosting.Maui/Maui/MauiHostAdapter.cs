using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.Maui;

public class MauiHostAdapter : IHost
{
    private MauiApp app;

    public MauiHostAdapter(MauiApp mauiApp)
    {
        app = mauiApp;
    }

    public IServiceProvider Services => app.Services;

    public void Dispose() => app.Dispose();

    public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}