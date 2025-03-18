using LionFire.Hosting;
using LionFire.Persistence;
using LionFire.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{
    public static class FilesystemTestHost
    {
        public static IHostBuilder Create()
        {
            return PersistersHost.Create()
                .ConfigureServices((_, services) =>
                {
                    services.AddFilesystemPersister();
                });
        }
    }
    public static class NewtonsoftJsonFilesystemTestHost
    {
        public static IHostBuilder Create()
        {
            return PersistersHost.Create()
              .ConfigureServices(s =>
              {
                  s
                  .AddNewtonsoftJson()
                  .AddFilesystemPersister()
                  ;
              });
        }
    }
}
