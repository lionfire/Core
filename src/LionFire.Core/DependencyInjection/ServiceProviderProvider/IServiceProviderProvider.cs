using System;

namespace LionFire.DependencyInjection
{
    public interface IServiceProviderProvider
    {
        IServiceProvider GetServiceProvider(object obj);
    }


}
