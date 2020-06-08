using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Applications.Splash
{
    public interface ISplashView : IHostedService, IDisposable
    {
        ISplashService SplashService { set; }
    }
}
