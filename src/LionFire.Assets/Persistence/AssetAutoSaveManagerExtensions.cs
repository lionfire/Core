using LionFire.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LionFire.ExtensionMethods;
using LionFire.Structures;
using LionFire.Results;

namespace LionFire.Persistence
{


    public static class AssetToSaveable // TODO: Move this capability off of Asset to more generic interfaces
    {
        #region Wrappers

        public static ICommitable ToSaveable(this IAsset asset)
        {
            return wrappers.GetOrAdd(asset, a => new SaveableAssetWrapper
            {
                Asset = a,
            });
        }
        public static ICommitable TryGetSaveable(this IAsset asset)
        {
            return wrappers.TryGetValue(asset);
        }

        private static ConcurrentDictionary<IAsset, ICommitable> wrappers = new ConcurrentDictionary<IAsset, ICommitable>();


        private class SaveableAssetWrapper : ICommitable, IReadWrapper<object>
        {
            public IAsset Asset { get; set; }
            object IReadWrapper<object>.Object => Asset;

            async Task<ICommitResult> ICommitable.Commit()
            {
                await Asset.SaveAtSubPath(Asset.AssetSubPath);
                return (ICommitResult)SuccessResult.Success; // REVIEW - Fix this idea
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
            Action<object> saveAction = o => ((ICommitable)o).Commit(); // TODO: wrap the Commit with auto-retry logic
            if (enable)
            {
                asset.ToSaveable().EnableAutoSave(true, saveAction);
            }
            else
            {
                ICommitable saveable;
                if (wrappers.TryRemove(asset, out saveable))
                {
                    saveable.EnableAutoSave(false, saveAction);
                }
            }
        }
    }
}
