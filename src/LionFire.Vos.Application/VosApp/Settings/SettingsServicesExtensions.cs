using LionFire.Applications;
using LionFire.Services;
using LionFire.Hosting;
using LionFire.Vos;
using LionFire.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.DependencyMachines;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using LionFire.Extensions.DefaultValues;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace LionFire.Settings
{

    public static class SettingsServicesExtensions
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, bool mountAll = true)
        => services
            .VobEnvironment("AppSettingsId", serviceProvider => serviceProvider.GetService<AppInfo>().AppId)

            .VobEnvironment("Settings", "/Settings".ToVobReference())

        #region Dependency: Accounts  (TODO: Only add if needed)
            .VobEnvironment("AccountSettings", "$Account/Settings".ToVobReference())
            .VobEnvironment("AccountAppSettings", "$Account/Settings/$AppSettingsId".ToVobReference())
        #endregion

            // c:\Users\user\AppData\Roaming\LionFire\AppId\Settings
            .VobEnvironment("UserSettings", "$Settings/User".ToVobReference()) // prefer accountSettings if available (and sync to cloud)

            // c:\Users\user\AppData\Local\LionFire\AppId\Settings
            .VobEnvironment("UserLocalSettings", "$Settings/UserLocal".ToVobReference()) // Prefer userSettings unless there's a good reason

            // c:\ProgramData\LionFire\AppId\Settings
            .VobEnvironment("AppLocalSettings", "$Settings/AppLocal".ToVobReference())

            .If(mountAll, s => s
                .MountUserSettings()
                .MountUserLocalSettings()
                .MountAppLocalSettings()
            )

            .AddSingletonHostedServiceDependency<SettingsManager>(c =>
            {
                c
                  .DependsOn("vos:/")
                  .Contributes("vos:$Settings")
                  ;
            })

           .Configure<SettingsOptions>(o =>
           {
               o.AutoSave = true;
               o.SaveOnExit = true;
           })

            .AddSingleton(typeof(IUserLocalSettings<>), typeof(UserLocalSettingsProvider<>))
            ;

        public static IServiceCollection MountUserSettings(this IServiceCollection services)
            => services
                .VosMountReadWrite("$Settings/User", "$stores/RoamingAppDataAppDir/Settings".ToVobReference())
            ;
        public static IServiceCollection MountUserLocalSettings(this IServiceCollection services)
         => services
             .VosMountReadWrite("$Settings/UserLocal", "$stores/LocalAppDataAppDir/Settings".ToVobReference())
         ;
        public static IServiceCollection MountAppLocalSettings(this IServiceCollection services)
           => services
               .VosMountReadWrite("$Settings/AppLocal", "$stores/ProgramDataAppDir/Settings".ToVobReference())
           ;
    }
}
