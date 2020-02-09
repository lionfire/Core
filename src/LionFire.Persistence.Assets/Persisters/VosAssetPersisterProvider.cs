using LionFire.Assets;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Assets.Persisters
{
    public class VosAssetPersisterProvider : IPersisterProvider<IAssetReference>
    {
        IRootManager RootManager { get; }

        public VosAssetPersisterProvider(IRootManager rootManager)
        {
            RootManager = rootManager;
        }

        public bool HasDefaultPersister => throw new NotImplementedException();

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// Vob path from which to locate the next available AssetManager.
        ///
        /// Examples:
        ///  - /path/to/assetManager
        ///  - /deep/path/to/object/but/will/use/ancestors'/assetManager
        ///  - /../otherRootVob/location/to/vob
        /// </param>
        /// <returns></returns>
        public IPersister<IAssetReference> GetPersister(string name = null) => RootManager.GetVob(name).GetOwn<VosAssetPersister>();
    }

    //public class VAssetPersister
    //{
    //    public Vob AssetRoot { get; private set; }
    //    public IPersister<ITypedReference> Persister { get; private set; }
    //}
}
