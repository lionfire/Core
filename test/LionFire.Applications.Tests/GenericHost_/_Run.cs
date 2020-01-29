using LionFire.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.Applications.Hosting;
using System.Threading;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GenericHost_
{
    // TODO: Do something useful during the Run

    public class _RunAndExit
    {
        [Fact]
        public async void Pass_Action_Services()
        {
            await FrameworkHostBuilder.Create()
                .RunAsync(services =>
                {
                    services.GetService<ILogger<_RunAndExit>>().LogInformation("test log");
                    //Thread.Sleep(10);
                    Debug.WriteLine("test run options action 2");
                });
        }

        [Fact]
        public async void Pass_Action()
        {
            await FrameworkHostBuilder.Create().RunAsync(() => {
                //Thread.Sleep(40);
                Debug.WriteLine("Pass_Action"); });
        }

        [Fact]
        public async void Pass_Task()
        {
            await FrameworkHostBuilder.Create()
                .RunAsync(services => Task.Run(async () =>
                {
                    Debug.WriteLine("test run options action 1 ");
                    services.GetService<ILogger<_RunAndExit>>().LogInformation("test log");
                    await Task.Delay(5);
                    Debug.WriteLine("test run options action 2");
                }));
        }

        [Fact]
        public async void Fail_Action_Services()
        {
            await FrameworkHostBuilder.Create()
                .RunAsync(services =>
                {
                    services.GetService<ILogger<_RunAndExit>>().LogInformation("test log");
                    //Thread.Sleep(10);
                    Assert.ThrowsAsync<Exception>(() => throw new Exception("test fail"));
                });
        }

        [Fact]
        public async void Fail_Action()
        {
            await FrameworkHostBuilder.Create().RunAsync(() => {
                //Thread.Sleep(40);
                Assert.ThrowsAsync<Exception>(()=> throw new Exception("test fail"));
            });
        }

        [Fact]
        public async void Fail_Task()
        {
            await FrameworkHostBuilder.Create()
                .RunAsync(services => Task.Run(async () =>
                {
                    Debug.WriteLine("test run options action 1 ");
                    services.GetService<ILogger<_RunAndExit>>().LogInformation("test log");
                    await Task.Delay(5);
                    await Assert.ThrowsAsync<Exception>(() => throw new Exception("test fail"));
                }));
        }
    }
}
