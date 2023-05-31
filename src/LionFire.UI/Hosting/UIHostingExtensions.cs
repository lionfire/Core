//using LionFire.Shell;
//using LionFire.Shell.Wpf;
//using LionFire.UI;
//using Microsoft.Extensions.DependencyInjection;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.Hosting;

//public static class UIHostingX
//{
//    public static IHostBuilder UseUI(this IHostBuilder hostBuilder, Action<UIHostingBuilder> configure = null)
//    {
//        hostBuilder.ConfigureServices((context, services) =>
//        {
//            services.AddUI();
//        });

//        return hostBuilder;
//    }

//    public static IServiceCollection AddUI(this IServiceCollection services)
//    {
//        services.AddSingleton<IShellProvider, WpfShellProvider>();
//        services.AddSingleton<IShell, WpfShell>();
//        services.AddSingleton<IShellViewModel, ShellViewModel>();
//        services.AddSingleton<IShellView, ShellView>();

//        return services;
//    }
//}
//{
    
//}
