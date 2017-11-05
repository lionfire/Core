using LionFire.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LionFire.ExtensionMethods;
using LionFire.Structures;

namespace LionFire.Persistence
{
    

    public static class AssetToSaveable
    {
        #region Wrappers

        public static ISaveable ToSaveable(this IAsset asset)
        {
            return wrappers.GetOrAdd(asset, a => new SaveableAssetWrapper
            {
                Asset = a,
            });
        }
        public static ISaveable TryGetSaveable(this IAsset asset)
        {
            return wrappers.TryGetValue(asset);
        }

        private static ConcurrentDictionary<IAsset, ISaveable> wrappers = new ConcurrentDictionary<IAsset, ISaveable>();


        private class SaveableAssetWrapper : ISaveable, IWrapper
        {
            public IAsset Asset { get; set; }
            object IWrapper.WrapperTarget=> Asset;

            Task ISaveable.Save(object context)
            {
                Asset.Save(Asset.AssetSubPath);
                return Task.CompletedTask;
            }
        }

        #endregion

        public static void QueueAutoSave(this IAsset asset)
        {
            var wrapper = wrappers.TryGetValue(asset);
            if (wrapper != null) wrapper.QueueAutoSave();
        }

        public static void EnableAutoSave(this IAsset asset, bool enable = true)
        {
            Action<object> saveAction = o => ((ISaveable)o).Save();
            if (enable)
            {
                asset.ToSaveable().EnableAutoSave(true, saveAction);
            }
            else
            {
                ISaveable saveable;
                if (wrappers.TryRemove(asset, out saveable))
                {
                    saveable.EnableAutoSave(false, saveAction);
                }
            }
        }
    }
}
