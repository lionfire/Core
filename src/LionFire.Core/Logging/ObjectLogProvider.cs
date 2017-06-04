using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MSLogger = Microsoft.Extensions.Logging.ILogger;
using LionFire.DependencyInjection;

namespace LionFire.Extensions.Logging
{
    /// <summary>
    /// Setup: ((IAppHost)app).AddProvider<ObjectLogProvider>() or is there a better way?  Just Add<ObjectLogProvider>()?
    /// </summary>
    public class ObjectLogProvider : IProvider<ILogger>, IConfigures<IServiceCollection>
    {
        public void Configure(IServiceCollection context)
        {
            context.AddSingleton<IProvider<ILogger>>(this);
        }

        public MSLogger ProvideForObject(object obj)
        {
            return obj.GetLogger();
        }
    }
  
}
