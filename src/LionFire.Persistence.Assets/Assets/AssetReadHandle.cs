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

        //object IReadHandle.Object { get { return Object; } }
        public TAsset Object
        {
            get
            {
                if (!isLoaded)
                {
                    _Load();
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

        private void _Load()
        {
            obj = assetSubPath.Load<TAsset>();
            isLoaded = true;
        }

        public async Task<bool> Initialize()
        {
            return await Task.Factory.StartNew(() =>
            {
                _Load();
                
                return obj != null;
            }).ConfigureAwait(false);
        }

        public bool HasObject { get { return Object != null; } }

    }
}
