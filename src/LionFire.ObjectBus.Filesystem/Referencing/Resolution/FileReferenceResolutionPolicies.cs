using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing.Persistence;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.Referencing.Filesystem
{

    public class FileReferenceResolutionPolicies // : PathReferenceResolutionPolicy
    {
        public static ReferenceResolutionPolicy Default = new ReferenceResolutionPolicy()
        {
            ReferenceResolutionService = new ReferenceResolutionService(new List<IReferenceResolutionStrategy>
            {
                Singleton<ExactReferenceResolutionStrategy>.Instance
            })
        };

        public static ReferenceResolutionPolicy Recommended = new ReferenceResolutionPolicy()
        {
            ReferenceResolutionService = new ReferenceResolutionService(new List<IReferenceResolutionStrategy>
            {
                Singleton<ExactReferenceResolutionStrategy>.Instance,
                //ManualSingleton<ExtensionlessReferenceResolutionService>.GetGuaranteedInstance<ExtensionlessReferenceResolutionService>(()=>new ExtensionlessReferenceResolutionService(FsOBase.Instance)),
            })
        };

    }

}
