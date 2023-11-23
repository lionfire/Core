#nullable enable
using LionFire.Dependencies;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Data.Id;

namespace LionFire.Vos.Id.Persisters
{
    public class VosIdPersisterProvider : IPersisterProvider<IIdReference>
    {
        IVos RootManager { get; }

        public VosIdPersisterProvider(IVos rootManager)
        {
            RootManager = rootManager;
        }

        public bool HasDefaultPersister => throw new NotImplementedException();

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// Vob path from which to locate the next available IdedManager.
        ///
        /// Examples:
        ///  - /path/to/assetManager
        ///  - /deep/path/to/object/but/will/use/ancestors'/assetManager
        ///  - /../otherRootVob/location/to/vob
        /// </param>
        /// <returns></returns>
        public IPersister<IIdReference> GetPersister(string? name = null) 
            => RootManager.Get(name)?.AcquireOwn<VosIdPersister>()  // TODO: Get VosIdPersister from vos:$id instead of vos:/.
                ?? throw new DependencyMissingException($"Missing VosIdedPersister on {(name == null ? "primary VobRoot" : $"VobRoot '{name}'")}");
    }
}
