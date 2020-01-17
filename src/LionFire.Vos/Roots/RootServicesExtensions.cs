using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Services
{
    public static class RootServicesExtensions
    {
        public static IServiceCollection AddVosRoot(IServiceCollection services, string name = null)
            => services.AddSingleton(new RootRegistration(name));
       
    }
}

