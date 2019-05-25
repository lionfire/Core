//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages

using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    public class StoreMounter
    {
        public static IEnumerable<IReference> Locations
        {
            get
            {
                yield return VosDiskPaths.UserSharedStoresRoot.AsFileReference();
            }
        }
        public static IEnumerable<IReference> GlobalLocations
        {
            get
            {
#if UNITY
                yield break;
#else
                yield return VosDiskPaths.GlobalSharedStoresRoot.AsFileReference();

#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="createIfMissing"></param>
        /// <param name="mountAsStoreName"></param>
        /// <param name="useGlobal">FUTURE: If true, use or create the global store.  If false, use or create the user store.  If null, load either or create user if none exists.</param>
        /// <returns></returns>
        public static Mount Mount(string storeName, bool createIfMissing = true, string mountAsStoreName = null, bool? useGlobal = null)
        {
            if (mountAsStoreName == null)
            {
                mountAsStoreName = storeName;
            }

            Mount mount = null;

            IEnumerable<IReference> locations;

            if (useGlobal.HasValue)
            {
                locations = useGlobal.Value ? GlobalLocations : Locations;
            }
            else
            {
                locations = Locations.Concat(GlobalLocations).Distinct();
            }

            foreach (var location in locations)
            {
                var storeLocation = location.GetChildSubpath(storeName);
                var storeMetaLocation = storeLocation.GetChildSubpath(StoreMetadata.DefaultName);
                var hStore = storeMetaLocation.GetHandle<StoreMetadata>();
                var metadata = hStore.Object;
                if (metadata != null)
                {
                    l.Info("[store] Found store at " + location);

                    mount = new Mount(V.Stores[mountAsStoreName], storeLocation, store: storeName, enable: true, mountOptions: new MountOptions()
                    {
                        IsExclusive = true,
                    });
                }
            }

            if (mount == null && createIfMissing)
            {
                foreach (var location in locations)
                {
                    try
                    {
                        var storeLocation = location.GetChildSubpath(storeName);
                        var storeMetaLocation = storeLocation.GetChildSubpath(StoreMetadata.DefaultName);
                        var hStore = storeMetaLocation.GetHandle<StoreMetadata>();
                        var metadata = new StoreMetadata();

                        hStore.Object = metadata;
                        hStore.Save();

                        mount = new Mount(V.Stores[mountAsStoreName], storeLocation, store: storeName, enable: true, mountOptions: new MountOptions()
                        {
                            IsExclusive = true,
                        });
                        break;
                    }
                    catch (Exception)
                    {
                        l.Error("Failed to create mount at " + location);
                    }
                }
            }

            return mount;
        }

        //public static Mount Mount(LocalFileReference fileReference, bool createIfMissing = true, string mountAsStoreName = null, bool? useGlobal = null)
        //{

        //    return null;

        //}

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

}

