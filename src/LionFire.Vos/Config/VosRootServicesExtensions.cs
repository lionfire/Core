using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Vos
{
    public static class VosRootServicesExtensions
    {
        public static IServiceCollection AddVosRoot(IServiceCollection services, string name = null)
            => services.AddSingleton(new VosRootRegistration(name));
    }
}
