// TODO?
//using LionFire.Execution;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LionFire.Assets
//{
//    public class AssetInterfaceReadHandle<TAssetInterface> : ReadHandleBase<TAssetInterface>, IReadHandle<TAssetInterface>
//        where TAssetInterface : class
//    {
//        [SetOnce]
//        public string AssetTypeName { get => Key; set => Key = value; }

//        string assetSubPath;

//        public AssetInterfaceReadHandle(string assetTypeName, string assetSubPath)
//        {
//            this.AssetTypeName = assetTypeName;
//            this.assetSubPath = assetSubPath;
//        }

//        #region Object

//        //object IReadHandle.Object { get { return Object; } }
//        public TAssetInterface Object
//        {
//            get
//            {
//                if (!isLoaded)
//                {
//                    Initialize().Wait();
//                }
//                return obj;
//            }
//        }
//        private TAssetInterface obj;
//        private bool isLoaded = false;

//        #endregion

//        #region Utility Methods


//        public AssetInterfaceReadHandle<TAssetInterface> Clone()
//        {
//            return new AssetInterfaceReadHandle<TAssetInterface>(AssetTypeName, assetSubPath);
//        }
        
//        #endregion


//        public async Task<bool> Initialize()
//        {
//            return await Task.Factory.StartNew(() =>
//            {
//                obj = assetSubPath.Load<TAssetInterface>(AssetTypeName);
//                isLoaded = true;
//                return obj != null;
//            }).ConfigureAwait(false);
//        }

//        public bool HasObject { get { return Object != null; } }

//    }
//}
