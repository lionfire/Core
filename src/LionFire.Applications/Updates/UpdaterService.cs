#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif


namespace LionFire.Applications.Updates
{
    public class UpdaterService
    {
        public bool FirstRunWithNewVersion { get; private set; }
    }
}
