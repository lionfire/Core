using System;
using System.Collections.Generic;
using LionFire.DependencyInjection;
using LionFire.ObjectBus.Filesystem;
using LionFire.Structures;
using LionFire.Persistence.Resolution;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    // FUTURE: Do DI with references to ExtensionlessReferenceResolutionService and in turn serialization strategies.  For now, use singletons.

    public class ExtensionlessFSOBase : OverlayOBase<ExtensionlessFileReference, FileReference>, IOverlayOBase<ExtensionlessFileReference, FileReference>
    {
        #region Static

        public static ExtensionlessFSOBase Instance => DependencyContext.Default.GetServiceOrSingleton<ExtensionlessFSOBase>();

        #endregion

        #region Constants

        public override IEnumerable<string> UriSchemes
        {
            get
            {
                // REFACTOR - same code as OBase?
                yield return ExtensionlessFileReference.Constants.UriScheme;
            }
        }

        #endregion

        #region Relationships

        public override IOBus OBus => ManualSingleton<ExtensionlessFSOBus>.GuaranteedInstance;

        public override IOBase UnderlyingOBase => FsOBase.Instance;

        #endregion

        #region Parameters

        public override IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }

        #endregion

        #region Construction

        public ExtensionlessFSOBase(IServiceProvider serviceProvider)
        {
            this.ReferenceResolutionStrategy = ActivatorUtilities.CreateInstance<ExtensionlessReferenceResolutionService>(serviceProvider, UnderlyingOBase);
            //this.ReferenceResolutionStrategy = new ExtensionlessReferenceResolutionService(UnderlyingOBase, SerializationProvider);
        }

        public ExtensionlessFSOBase(ExtensionlessReferenceResolutionService resolutionService)
        {
            this.ReferenceResolutionStrategy = resolutionService;
        }

        #endregion

    }
}
