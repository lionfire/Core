using LionFire.Hosting.Maui;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;

namespace LionFire.Hosting;

public static class MauiHostingExtensions
{
    public static MauiAppBuilder LionFire(this MauiAppBuilder mauiAppBuilder, Action<LionFireHostBuilder>? action = null, bool useDefaults = true)
    {
        var lf = new LionFireHostBuilder(new MauiHostBuilderAdapter(mauiAppBuilder));

        if (useDefaults) { lf.Defaults(); }

        action?.Invoke(lf);

        return mauiAppBuilder;
    }
}