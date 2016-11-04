using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public class AssetReadHandle<TAsset> : IReadHandle<TAsset>
        where TAsset : class
    {
        string assetSubPath;

        public AssetReadHandle(string assetSubPath)
        {
            this.assetSubPath = assetSubPath;
        }

        object IReadHandle.Object { get { return Object; } }
        public TAsset Object
        {
            get
            {
                if (!isLoaded)
                {
                    Initialize().Wait();
                }
                return obj;
            }
        }
        private TAsset obj;
        private bool isLoaded = false;

        public AssetReadHandle<TAsset> Clone()
        {
            return new AssetReadHandle<TAsset>(assetSubPath);
        }

        public async Task<bool> Initialize()
        {
            return await Task.Factory.StartNew(() =>
            {
                obj = assetSubPath.Load<TAsset>();
                isLoaded = true;
                return obj != null;
            });
        }

        public bool HasObject { get { return Object != null; } }

    }
}
