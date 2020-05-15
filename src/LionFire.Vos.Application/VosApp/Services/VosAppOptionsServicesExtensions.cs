//using LionFire.Vos.VosApp;
//using Microsoft.Extensions.DependencyInjection;

//namespace LionFire.Services
//{
//    public static class VosAppOptionsServicesExtensions
//    {
//        public static IServiceCollection AddVosAppOptions(this IServiceCollection services, VosAppOptions options)
//        {
//            foreach (var packageProvider in options.PackageProviders)
//            {
//                services.AddPackageProvider(packageProvider.Key, packageProvider.Value);
//            }

//            services.AddSingleton(options); // UNUSED?

//            return services;
//        }
//    }
//}
