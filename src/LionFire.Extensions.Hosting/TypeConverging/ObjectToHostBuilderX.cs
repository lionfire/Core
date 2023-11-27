using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class ObjectToHostBuilderX
{
    public static Action<Action<IServiceCollection>> GetConfigureServices(object? builder)
    {
        if (builder is IHostBuilder hb) return a => hb.ConfigureServices(a);
        else if (builder is IHostApplicationBuilder hab) return a => a(hab.Services);
        else throw new NotSupportedException($"Type: {builder?.GetType().FullName}");
    }
    public static IDictionary<object, object> GetProperties(object? builder)
    {
        if (builder is IHostBuilder hb) return hb.Properties;
        else if (builder is IHostApplicationBuilder hab) return hab.Properties;
        else throw new NotSupportedException($"Type: {builder?.GetType().FullName}");
    }
}
