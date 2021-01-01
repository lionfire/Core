#nullable enable
using LionFire.Assets;
using LionFire.Dependencies;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Assets.Persisters
{
    /// <summary>
    /// Provides an accessor for VosAssetPersister, which is stored as a Node on IRootVobs, accessible via AcquireOwn&lt;VosAssetPersister&gt;.
    /// </summary>
    public class VosAssetPersisterProvider : IPersisterProvider<IAssetReference>
    {
        IVos RootManager { get; }

        public VosAssetPersisterProvider(IVos rootManager)
        {
            RootManager = rootManager;
        }

        public bool HasDefaultPersister => throw new NotImplementedException();

        /// <summary>
        /// </summary>
        /// <param name="rootName">
        /// Vob path from which to locate the next available AssetManager.
        ///
        /// Examples:
        ///  - /path/to/assetManager
        ///  - /deep/path/to/object/but/will/use/ancestors'/assetManager
        ///  - /../otherRootVob/location/to/vob
        /// </param>
        /// <returns></returns>
        public VosAssetPersister GetPersister(string? rootName = null) 
            => RootManager.Get(rootName)?.AcquireOwn<VosAssetPersister>() 
                ?? throw new DependencyMissingException($"Missing VosAssetPersister on {(rootName == null ? "primary VobRoot" : $"VobRoot '{rootName}'")}");

        IPersister<IAssetReference> IPersisterProvider<IAssetReference>.GetPersister(string? rootName) => GetPersister(rootName);
    }

}
