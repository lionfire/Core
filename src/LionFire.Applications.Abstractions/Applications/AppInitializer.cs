
using LionFire.Applications.Hosting;
using LionFire.Execution;
using LionFire.Structures;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public class AppInitializer : IInitializable3
    {

        public Func<IAppHost, Task<object>> InitMethod { get; set; }

        #region Construction

        public AppInitializer(Func<IAppHost, Task<object>> tryInitMethod) { this.InitMethod = (app) => tryInitMethod(app); }
        public AppInitializer(Func<IAppHost, object> tryInitMethod) { this.InitMethod = (app) => Task.FromResult(tryInitMethod(app)); }
        public AppInitializer(Action<IAppHost> initMethod) { this.InitMethod = app => { initMethod(app); return Task.FromResult<object>(null); }; }

        #endregion

        public async Task<object> Initialize()
        {
            return await InitMethod(ManualSingleton<IAppHost>.Instance).ConfigureAwait(false);
        }
    }
}

