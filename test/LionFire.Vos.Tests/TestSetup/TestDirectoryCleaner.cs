using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace LionFire.Hosting;

public class TestDirectoryCleaner : IHostedService
{
    public TestDirectoryCleaner(string dirToDelete, IHostApplicationLifetime al)
    {
        al.ApplicationStopping.Register(() =>
        {
            try
            {
                Directory.Delete(dirToDelete, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to clean up test directory: " + dirToDelete, ex);
            }
        });
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) { 
        return Task.CompletedTask;
    }
}
