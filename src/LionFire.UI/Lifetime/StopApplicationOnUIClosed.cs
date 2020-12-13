using Microsoft.Extensions.Hosting;
//using AsyncEx;
using LionFire.Execution;
using LionFire.Threading;
using System.Threading.Tasks;
using TypeSupport.Extensions;

namespace LionFire.UI
{
    public class StopApplicationOnUIClosed
    {
        public StopApplicationOnUIClosed(IUILifetime uiLifetime, IHostApplicationLifetime hostApplicationLifetime)
        {
            Task.Run(async () =>
            {
                await uiLifetime.Stopped.WaitHandle.WaitOneAsync().ConfigureAwait(false);
                hostApplicationLifetime.StopApplication();
                //uiLifetime.Closed += _ => hostApplicationLifetime.StopApplication();
            });
        }
    }
}

