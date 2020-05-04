#if UNUSED // Not sure if this is useful
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LionFire.Hosting.Unity
{

    // Unity as a library:      
    //  - Supported on: Android, iOS, Windows, UWP
    //  - https://docs.unity3d.com/Manual/UnityasaLibrary.html

    public class UnityLifetime : IHostLifetime
    {
        //public IHostApplicationLifetime HostApplicationLifetime { get; }

        //public UnityLifetime(IHostApplicationLifetime hostApplicationLifetime)
        //{
        //    //HostApplicationLifetime = hostApplicationLifetime;
        //}

        public  Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            //HostApplicationLifetime.ApplicationStopped.Register(state =>
            //{
            //    ((TaskCompletionSource<object>)state).TrySetResult(null);
            //}, taskCompletionSource);

            //Application.Quit(); // Ignored in editor
            //await taskCompletionSource.Task.ConfigureAwait(false);
        }

        public  Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            //HostApplicationLifetime.ApplicationStarted.Register(state =>
            //{
            //    ((TaskCompletionSource<object>)state).TrySetResult(null);
            //}, taskCompletionSource);

            //await taskCompletionSource.Task.ConfigureAwait(false);
        }
    }

    public static class UnityIHostBuilderExtensions
    {
        public static IHostBuilder UseUnityLifetime(this IHostBuilder hostBuilder) 
            => hostBuilder.ConfigureServices((context, services) => services.AddSingleton<IHostLifetime, UnityLifetime>());
    }

}
#endif