using System;

namespace LionFire.Dependencies
{
    public interface IServiceProviderProvider
    {
        IServiceProvider GetServiceProvider(object obj);
    }


}
