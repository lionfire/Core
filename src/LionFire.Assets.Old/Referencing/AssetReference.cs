using System;
using System.Collections.Generic;
using LionFire.Referencing;

namespace LionFire.Assets
{
    public class AssetReference<T> : LocalReferenceBase<AssetReference<T>>
        where T : class
    {
        public override string Scheme => "asset";
        public override IEnumerable<string> AllowedSchemes
        {
            get
            {
                yield return Scheme;
            }
        }
        public override string Key { get => Path; protected set => Path = value; }


        #region AssetSubPath

        public string AssetSubPath
        {
            get => assetSubPath;
            set
            {
                if (assetSubPath == value)
                {
                    return;
                }

                if (assetSubPath != default(string))
                {
                    throw new AlreadySetException();
                }

                assetSubPath = value;
            }
        }

        private string assetSubPath;

        public override string Path {
            get => typeof(T).Name + ReferenceConstants.PathSeparator + AssetSubPath;
            set => throw new NotImplementedException();
        }

        #endregion

        public AssetReference() { }
        public AssetReference(string assetSubPath)
        {
            AssetSubPath = assetSubPath;
        }
        public AssetReference(T asset) {
            throw new Exception("Probably broken. Verify.");
            //AssetSubPath = asset.ID.Path;
        }
        public AssetReference(AssetID assetId)
        {
            throw new Exception("Probably broken. Verify.");
            AssetSubPath = assetId.Path;
        }
    }

    //public class AssetReadHandle<T> : IReadHandle<T>
    //{
    //    string assetSubPath;
    //    public AssetReadHandle(string assetSubPath)
    //    {
    //        this.assetSubPath = assetSubPath;
    //    }

    //    public T Object
    //    {
    //        get
    //        {
    //            if (obj == null)
    //            {
    //                obj = assetSubPath.Load<T>();
    //            }
    //            return obj;
    //        }
    //    }
    //    private T obj;
    //    public bool HasObject { get { return } }
    //}

    //public static class ReadHandleExtensions
    //{
    //    public static IReadHandle<T> Handle<T>(this string assetSubPath)
    //    {
    //        return new AssetReadHandle<T>(assetSubPath);
    //    }
    //}
}
