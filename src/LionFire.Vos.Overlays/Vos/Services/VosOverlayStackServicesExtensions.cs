using LionFire.MultiTyping;
using LionFire.Vos;
using LionFire.Vos.Overlays;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VosOverlayStackServicesExtensions
    {
        public static IServiceCollection VosOverlayStack(this IServiceCollection services, IVosReference vosReference, OverlayStackOptions options = null)
        {
            services.InitializeVob(vosReference, (serviceProvider, v) =>
             {
                 v.AddOverlayStack(options);
             });
            return services;
        }
        public static IServiceCollection VosOverlayStack(this IServiceCollection services, string vosPath, OverlayStackOptions options = null)
            => services.VosOverlayStack(vosPath.ToVosReference(), options);
    }
}
