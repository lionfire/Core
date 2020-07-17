using LionFire.Applications;
using LionFire.Services;
using LionFire.Services.DependencyMachines;
using LionFire.Vos;
using LionFire.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Settings
{
    public static class SettingsServicesExtensions
    {
        public static IServiceCollection AddSettings(this IServiceCollection services)
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
            .VobEnvironment("AppSettings", "$Settings/app".ToVobReference())

            .VobEnvironment("Settings", "$Settings/".ToVobReference())

            .AddHostedServiceDependency<SettingsManager>(c =>
            {


            })
            ;
    }
}
