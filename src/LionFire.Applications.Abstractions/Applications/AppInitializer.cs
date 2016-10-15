using LionFire.Applications.Hosting;
using LionFire.Execution;
using LionFire.Structures;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public class AppInitializer : IInitializable
    {

        public Func<IAppHost, bool> InitMethod { get; set; }

        #region Construction

        public AppInitializer(Func<IAppHost, bool> tryInitMethod) { this.InitMethod = tryInitMethod; }
        public AppInitializer(Action<IAppHost> initMethod) { this.InitMethod = app => { initMethod(app); return true; }; }

        #endregion

        public async Task<bool> Initialize()
        {
            return InitMethod(ManualSingleton<IAppHost>.Instance);
        }
    }
}
