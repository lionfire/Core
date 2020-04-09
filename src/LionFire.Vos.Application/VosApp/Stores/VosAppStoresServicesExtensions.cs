using LionFire.Vos.Mounts;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem;
using LionFire.Vos;
using LionFire.Vos.Packages;
using System.IO;
using System.Reflection;
using LionFire.Environment.AutoDetect;
using LionFire.Applications;
using Microsoft.Extensions.Configuration;
using System;
using LionFire.FlexObjects;

namespace LionFire.Services
{

    public static class VosAppStoresServicesExtensions
    {
        public const string OrgName = nameof(AppInfo.OrgName);
        public const string AppName = nameof(AppInfo.AppName);
        public const string DataDirName = nameof(AppInfo.DataDirName);

        #region MOVE to LionFireEnvironment?

        // REVIEW these -- better way to get them?
        public static string ProgramDataDir => System.Environment.GetEnvironmentVariable("ProgramData"); // Or ALLUSERSPROFILE?

        public static string LocalAppDataDir => System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
        //public static string LocalAppDataDir => Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), "AppData\\Local");

        public static string LocalLowAppDataDir => Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), "AppData\\LocalLow");

        public static string RoamingAppDataDir => Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), "AppData\\Roaming");
        //public static string RoamingAppDataDir => System.Environment.GetEnvironmentVariable("APPDATA");

        #endregion

        // DELETE
        ///// <summary>
        /////  Uses default options: VosAppDefaults.Default.VosStoresOptions
        ///// </summary>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection AddVosStores(this IServiceCollection services, Action<IVosStoresBuilder> configurator = null)
        //    => services.AddVosStores(VosAppDefaults.Default.VosStoresOptions, configurator);


        // OLD TODELETE
        ///// <summary>
        ///// Add all default stores
        ///// </summary>
        ///// <param name="services"></param>
        ///// <param name="options"></param>
        ///// <returns></returns>
        //public static IServiceCollection AddVosAppStores(this IServiceCollection services, VosAppOptions options = null)
        //{
        //    if (options == null) options = new VosAppOptions();

        //    if (options.DefaultMountAppBase) services.VosMount(LionPath.Combine(VosPaths.Stores, VosStoreNames.AppBase), VosDiskPaths.AppBase.ToFileReference(), new MountOptions
        //    {
        //        IsExclusive = true,
        //        ReadPriority = 1,
        //    });


        //    services.AddVosStores(options ?? VosAppDefaults.Default.VosStoresOptions, stores =>
        //     {
        //         stores.AddStore("$stores/");
        //         stores.AddStore("/_/stores/Exe",);
        //     })

        //    return services;
        //}

        // Add parent csproj dir ?
        //public static IServiceCollection AddProjectDirStore(this IServiceCollection services, string storeName = VosAppStores.AppDir, string appId = null, bool onlyIfNotExeDir = true)
        //{
        // TODO
        //    var customApplicationDir = AutoDetectUtils.FindCustomAppRoot(appId);
        //    if (customApplicationDir == null) return services;

        //    var assembly = Assembly.GetEntryAssembly();
        //    if (!onlyIfNotExeDir && customApplicationDir == Path.GetDirectoryName(assembly?.Location)) return services;

        //    return services.VosMount("$stores/" + storeName, customApplicationDir.ToFileReference(), new MountOptions { });
        //}

        public static MountOptions DefaultOptions(string storeName, string path)
        {
            var result = new MountOptions();

            result.TryCreateIfMissing = storeName switch
            {
                StoreNames.ProgramDataOrgDir => false,
                _ => true,
            };

            if (path != null)
            {
                if (LionFireEnvironment.Directories.IsVariableDirectory != null)
                {
                    var isVar = LionFireEnvironment.Directories.IsVariableDirectory(path);
                    if (isVar.HasValue) result.IsVariableDataLocation = isVar.Value;
                }

                if (LionFireEnvironment.Directories.IsUserDirectory != null)
                {
                    var isUser = LionFireEnvironment.Directories.IsUserDirectory(path);
                    if (isUser.HasValue) result.IsOwnedByOperatingSystemUser = isUser.Value;
                }
            }

            return result;
        }

        public static IServiceCollection AddPlatformSpecificStores(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWindowsStores(configuration);
#if TODO
            services.AddLinuxStores(configuration);
            services.AddMacStores(configuration);
#endif
            return services;
        }

        #region Linux

        public static IServiceCollection AddLinuxStores(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        #endregion

        #region Mac

        public static IServiceCollection AddMacStores(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        #endregion

        #region Windows

        public static IServiceCollection AddWindowsStores(this IServiceCollection services, IConfiguration configuration)
        {
            // FUTURE ENH: Only mount if they exist and are used.  If installed to later, mount them later.

            return services
                .AddProgramDataOrgStore(configuration)
                .AddProgramDataAppStore(configuration)
                .AddProgramDataDataDirStore(configuration)

                .AddLocalAppDataOrgStore(configuration)
                .AddLocalAppDataAppStore(configuration)
                .AddLocalAppDataDataDirStore(configuration)

                .AddLocalLowAppDataOrgStore(configuration)
                .AddLocalLowAppDataAppStore(configuration)
                .AddLocalLowAppDataDataDirStore(configuration)

                .AddRoamingAppDataOrgStore(configuration)
                .AddRoamingAppDataAppStore(configuration)
                .AddRoamingAppDataDataDirStore(configuration);
        }

        #endregion

        #region RoamingAppData

        public static IServiceCollection AddRoamingAppDataDataDirStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
        {
            var path = Path.Combine(RoamingAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[DataDirName] ?? throw new ArgumentNullException(DataDirName));

            options ??= DefaultOptions(StoreNames.RoamingAppDataDataDir, path);
            options.IsOwnedByOperatingSystemUser = false;
            options.IsVariableDataLocation = true;

            return configuration[DataDirName] == null
                ? services
                : services.VosMount("$stores/" + StoreNames.RoamingAppDataDataDir,
               path.ToFileReference(),
               options);
        }

        public static IServiceCollection AddRoamingAppDataAppStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
        {
            var path = Path.Combine(RoamingAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[AppName] ?? throw new ArgumentNullException(AppName));

            options ??= DefaultOptions(StoreNames.RoamingAppDataDataDir, path);
            options.IsOwnedByOperatingSystemUser = false;
            options.IsVariableDataLocation = true;

            return services.VosMount("$stores/" + StoreNames.RoamingAppDataAppDir,
                 path.ToFileReference(),
                 options);
        }

        public static IServiceCollection AddRoamingAppDataOrgStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
        {
            var path = Path.Combine(RoamingAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(OrgName));

            options ??= DefaultOptions(StoreNames.RoamingAppDataDataDir, path);
            options.IsOwnedByOperatingSystemUser = false;
            options.IsVariableDataLocation = true;

            return services.VosMount("$stores/" + StoreNames.RoamingAppDataOrgDir,
                path.ToFileReference(),
                options);
        }

        #endregion

#error NEXT: Make below like the above.  Split out path and options, initialize DefaultOptions with path, and also explicitly  set Is flags on options.

        #region LocalLowAppData

        public static IServiceCollection AddLocalLowAppDataDataDirStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
           => configuration[DataDirName] == null
                ? services
                : services.VosMount("$stores/" + StoreNames.LocalLowAppDataDataDir,
               Path.Combine(LocalLowAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[DataDirName] ?? throw new ArgumentNullException(DataDirName)).ToFileReference(),
               options ?? DefaultOptions(StoreNames.LocalLowAppDataDataDir));

        public static IServiceCollection AddLocalLowAppDataAppStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
                => services.VosMount("$stores/" + StoreNames.LocalLowAppDataAppDir,
                    Path.Combine(LocalLowAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[AppName] ?? throw new ArgumentNullException(AppName)).ToFileReference(),
                    options ?? DefaultOptions(StoreNames.LocalLowAppDataAppDir));

        public static IServiceCollection AddLocalLowAppDataOrgStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
            => services.VosMount("$stores/" + StoreNames.LocalLowAppDataOrgDir,
                Path.Combine(LocalLowAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(OrgName)).ToFileReference(),
                options ?? DefaultOptions(StoreNames.LocalLowAppDataOrgDir));

        #endregion

        #region LocalAppData

        public static IServiceCollection AddLocalAppDataDataDirStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
           => configuration[DataDirName] == null
                ? services
                : services.VosMount("$stores/" + StoreNames.LocalAppDataDataDir,
               Path.Combine(LocalAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[DataDirName] ?? throw new ArgumentNullException(DataDirName)).ToFileReference(),
               options ?? DefaultOptions(StoreNames.LocalAppDataDataDir));

        public static IServiceCollection AddLocalAppDataAppStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
                => services.VosMount("$stores/" + StoreNames.LocalAppDataAppDir,
                    Path.Combine(LocalAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[AppName] ?? throw new ArgumentNullException(AppName)).ToFileReference(),
                    options ?? DefaultOptions(StoreNames.LocalAppDataAppDir));

        public static IServiceCollection AddLocalAppDataOrgStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
            => services.VosMount("$stores/" + StoreNames.LocalAppDataOrgDir,
                Path.Combine(LocalAppDataDir, configuration[OrgName] ?? throw new ArgumentNullException(OrgName)).ToFileReference(),
                options ?? DefaultOptions(StoreNames.LocalAppDataOrgDir));

        #endregion

        #region ProgramData

        public static IServiceCollection AddProgramDataDataDirStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
         =>
            configuration[DataDirName] == null
                ? services
                : services.VosMount("$stores/" + StoreNames.ProgramDataDataDir,
                   Path.Combine(ProgramDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[DataDirName] ?? throw new ArgumentNullException(DataDirName)).ToFileReference(),
                   options ?? DefaultOptions(StoreNames.ProgramDataDataDir));


        public static IServiceCollection AddProgramDataAppStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
                => services.VosMount("$stores/" + StoreNames.ProgramDataAppDir,
                    Path.Combine(ProgramDataDir, configuration[OrgName] ?? throw new ArgumentNullException(nameof(OrgName)), configuration[AppName] ?? throw new ArgumentNullException(AppName)).ToFileReference(),
                    options ?? DefaultOptions(StoreNames.ProgramDataAppDir));

        public static IServiceCollection AddProgramDataOrgStore(this IServiceCollection services, IConfiguration configuration, MountOptions options = null)
            => services.VosMount("$stores/" + StoreNames.ProgramDataOrgDir,
                Path.Combine(ProgramDataDir, configuration[OrgName] ?? throw new ArgumentNullException(OrgName)).ToFileReference(),
                options ?? DefaultOptions(StoreNames.ProgramDataOrgDir));

        #endregion

        public static IServiceCollection AddAppDirStore(this IServiceCollection services, string storeName = StoreNames.AppDir, AppInfo appInfo = null, bool onlyIfNotExeDir = false, MountOptions options = null, string appDirPath = null, bool useExeDirAsAppDirIfMissing = true)
        {
            if (appDirPath == null) appDirPath = ApplicationAutoDetection.FindCustomAppRoot(appInfo?.AppId);
            if (appDirPath == null && useExeDirAsAppDirIfMissing) appDirPath = ExeDirPath;
            if (appDirPath == null) return services;
            if (onlyIfNotExeDir && appDirPath == ExeDirPath) return services;

            options ??= DefaultOptions(storeName, appDirPath);
            return services.VosMount("$stores/" + storeName, appDirPath.ToFileReference(), options);
        }

        #region Default Stores

        public static string ExeDirPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null || assembly.Location == null) return null;

                var dir = Path.GetDirectoryName(assembly.Location);
                if (!Directory.Exists(dir)) return null;
                return dir;
            }
        }

        // REVIEW - move to LionFire.Vos.Overlays?  Can't now because can't create FileReferences from strings yet.
        public static IServiceCollection AddExeDirStore(this IServiceCollection services, string name = StoreNames.ExeDir)
        {
            var exeDirPath = ExeDirPath;
            if (exeDirPath != null)
            {
                services.VosMount("$stores/" + name, exeDirPath.ToFileReference(), new MountOptions { });
            }
            return services;
        }

        public static IServiceCollection AddExeDirBasePackage(this IServiceCollection services, string storeName = StoreNames.ExeDir)
        {
            return services.TryAddAvailablePackage("$DataPackageManager", $"$stores/{storeName}", new MountOptions(100, null));
            //return services.InitializeRootVob((serviceProvider, root) =>
            //{
            //    var packageManagerVob = "$DataPackageManager".QueryVob();
            //    if (packageManagerVob == null) return;
            //    var packageManager = packageManagerVob.AsPackageManager();
            //    if (packageManager == null) return;

            //    var target = $"$stores/{name}".QueryVob();
            //    if (target == null) return;

            //    packageManager?.AvailableRoot.Mount(target, new MountOptions(100, null));
            //});
        }

        public static IServiceCollection TryAddAvailablePackage(this IServiceCollection services, VosReference packageManager, VosReference target, MountOptions options = null)
        {
            return services.InitializeRootVob((serviceProvider, root) =>
            {
                var packageManagerVob = packageManager.QueryVob();
                if (packageManagerVob == null) return; // TOLOG
                var packageManagerObj = packageManagerVob.Get<PackageProvider>();
                if (packageManagerObj == null) return; // TOLOG

                var targetVob = target.QueryVob();
                if (targetVob == null) return; // TOLOG

                packageManagerObj?.AvailableRoot.Mount(targetVob, options);
            });
        }

        #endregion


    }
}
