using Microsoft.Extensions.DependencyInjection;
using System;
using LionFire.Dependencies;

namespace LionFire.Hosting
{

    public class LionFireDefaultServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        readonly DefaultServiceProviderFactory wrapped = new DefaultServiceProviderFactory();

        #region Construction

        public LionFireDefaultServiceProviderFactory()
        {
            wrapped = new DefaultServiceProviderFactory();
        }

        public LionFireDefaultServiceProviderFactory(ServiceProviderOptions options) //: base (options)
        {
            wrapped = new DefaultServiceProviderFactory(options);
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) => wrapped.CreateBuilder(services);

        #endregion

        public  IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var result = wrapped.CreateServiceProvider(containerBuilder);

            DependencyContext.AsyncLocal = new DependencyContext()
            {
                ServiceProvider = result,
            };

            return result;
        }
    }
}
