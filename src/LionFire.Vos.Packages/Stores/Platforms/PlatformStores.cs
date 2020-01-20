using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LionFire.Stores.Platforms
{
    
    public class PlatformStores
    {
#if true //OLD
        public virtual void InitPackageDirectories()
        {
            bool useVar = VosDiskPaths.AppBase != VosDiskPaths.VarBase;

            var packagePaths = new List<string>();  // Used to detect multiple registrations of the same path

            var packageDirectories = PackageMounter.PackageDirectories;

#region BasePacks

            packagePaths.Add(VosDiskPaths.AppBase);
            {
                var pd = new PackageDirectory()
                {
                    Path = VosDiskPaths.AppBase,
                    LocationName = VosStoreNames.AppBase,
                    MountOptions = new MountOptions()
                    {
                        IsReadOnly = true, // Base = readonly
                        ReadPriority = -1000,
                        WritePriority = 11000, // Shouldn't have permissions
                    },
                };
                packageDirectories.Add(pd);
            }

            if (useVar)
            {
                packagePaths.Add(VosDiskPaths.VarBase);
                {
                    var pd = new PackageDirectory()
                    {
                        Path = VosDiskPaths.VarBase,
                        LocationName = VosStoreNames.VarBase,
                        MountOptions = new MountOptions()
                        {
                            IsReadOnly = true, // Base = readonly
                            ReadPriority = -100,
                            // MaxValue for WritePriority means can only be written to if layer name is manually specified
                            WritePriority = 10500, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

#if ENABLE_APPDATA_FALLBACK
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.BasePacksPath_AppData,
                LayerName = DefaultMountOptions.AppDataLayerName,
                MountOptions = new MountOptions()
                {
                    IsReadOnly = true, // Base = readonly
                    ReadPriority = -50,
                    WritePriority = 10100, // Must have permissions
                },
            });
#endif

#endregion

#region UserPacks

            packagePaths.Add(VosDiskPaths.AppData);
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.AppData,
                LocationName = VosStoreNames.AppData,
                MountOptions = new MountOptions()
                {
                    TryCreateIfMissing = false,
                    IsReadOnly = true, // Shouldn't have permissions
                    ReadPriority = 100,
                    WritePriority = 1000,  // Shouldn't have permissions, unless user manually enabled this
                },
            });

            if (useVar)
            {
                packagePaths.Add(VosDiskPaths.VarData);
                {
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosDiskPaths.VarData,
                        LocationName = VosStoreNames.VarData,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = false, // May not have permissions
                            ReadPriority = 200,
                            WritePriority = 11000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

#if ENABLE_APPDATA_FALLBACK
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.UserPacksPath_AppData,
                LayerName = DefaultMountOptions.AppDataLayerName,
                MountOptions = new MountOptions()
                {
                    TryCreateIfMissing = true,
                    IsReadOnly = false, // Must have permissions
                    ReadPriority = 300,
                    WritePriority = 100, // Must have permissions
                },
            });
#endif

#endregion

#region Default Dir (changeable)

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomBaseDir))
            {
                if (packagePaths.Contains(VosConfiguration.CustomBaseDir))
                {
                    l.Trace("VosConfiguration.CustomBaseDir is already a package directory.");
                }
                else
                {
                    packagePaths.Add(VosConfiguration.CustomBaseDir);
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosConfiguration.CustomBaseDir,
                        LocationName = VosStoreNames.CustomBase,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = true,
                            ReadPriority = 2000,
                            WritePriority = 20000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomDataDir))
            {
                if (packagePaths.Contains(VosConfiguration.CustomDataDir))
                {
                    l.Trace("VosConfiguration.CustomDataDir is already a package directory.");
                }
                else
                {
                    packagePaths.Add(VosConfiguration.CustomDataDir);
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosConfiguration.CustomDataDir,
                        LocationName = VosStoreNames.CustomData,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = false, // May not have permissions
                            ReadPriority = 3000,
                            WritePriority = 12000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

#endregion

            var sb = new StringBuilder();

            sb.AppendLine(packageDirectories.Count + " package directories: ");
            foreach (var dir in packageDirectories)
            {
                sb.AppendLine(" - " + dir.Path);
            }
            l.Debug(sb.ToString());
        }

#endif

        public static TMount GetPlatformCommonStores()
        {

            return new TMount { Reference = EntryAssemblyLocation }
        }

        public static string EntryAssemblyLocation => Assembly.GetEntryAssembly().Location;

        public static GetPlatformSpecificStores()
        {
        //var desc = RuntimeInformation.OSDescription;
        //https://stackoverflow.com/questions/2819934/detect-windows-version-in-net/8406674
            //RuntimeInformation.FrameworkDescription;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    break;
                case PlatformID.Unix:
                    break;
                case PlatformID.Win32NT:

                    break;
                //case PlatformID.Win32S:
                    //break;
                //case PlatformID.Win32Windows:
                //    break;
                //case PlatformID.WinCE:
                //    break;
                //case PlatformID.Xbox:
                //    break;
                default:
                    break;
            }
        }
    }
}
