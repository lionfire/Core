using System;

namespace LionFire.Services.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ServiceHostCli().Run(args);
        }
    }
}
