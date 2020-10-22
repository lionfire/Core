#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using System;
using System.Threading.Tasks;

namespace LionFire.Applications.Updates
{
    public class NAppUpdaterService : UpdaterBase
    {
        public override Task<int> UpdatesAvailable
        {
            get
            {
                throw new NotImplementedException();
                //return UpdateManager.Instance.UpdatesAvailable;
            }
        }
    }
}
