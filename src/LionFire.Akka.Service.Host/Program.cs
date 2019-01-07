using System;

namespace LionFire.Akka.Service.Host
{
    class Program
    {
        public static async void Main(string[] args)
        {
            new HostBuilder()
                .UseHostedService<>()
                .Build()
                .Run();
        }
    }
}
