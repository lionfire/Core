using LionFire.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LionFire.ExtensionMethods;
using LionFire.Structures;
using LionFire.Results;
using LionFire.Data.Gets;

namespace LionFire.Persistence
{


    public static class AssetToSaveable // TODO: Move this capability off of Asset to more generic interfaces
    {
        #region Wrappers

        public static IPuts ToSaveable(this IAsset asset)
        {
            return wrappers.GetOrAdd(asset, a => new SaveableAssetWrapper
            {
                Asset = a,
            });
        }
        public static IPuts TryGetSaveable(this IAsset asset)
        {
            return wrappers.TryGetValue(asset);
        }

        private static ConcurrentDictionary<IAsset, IPuts> wrappers = new ConcurrentDictionary<IAsset, IPuts>();


        private class SaveableAssetWrapper : IPuts, IReadWrapper<object>
        {
            public IAsset Asset { get; set; }
            object IReadWrapper<object>.Value => Asset;

            async Task<ISuccessResult> IPuts.Put()
            {
                await Asset.SaveAtSubPath(Asset.AssetSubPath);
                return SuccessResult.Success;
            }
        }

        #endregion

        public static void QueueAutoSaveAsset(this IAsset asset)
        {
            var wrapper = wrappers.TryGetValue(asset);
            if (wrapper != null) wrapper.QueueAutoSave();
        }

        public static void EnableAutoSave(this IAsset asset, bool enable = true)
        {
            Action<object> saveAction = o => ((IPuts)o).Put(); // TODO: wrap the Put with auto-retry logic
            if (enable)
            {
                asset.ToSaveable().EnableAutoSave(true, saveAction);
            }
            else
            {
                IPuts saveable;
                if (wrappers.TryRemove(asset, out saveable))
                {
                    saveable.EnableAutoSave(false, saveAction);
                }
            }
        }
    }
}
