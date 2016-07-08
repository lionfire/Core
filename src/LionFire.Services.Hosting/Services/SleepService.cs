using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace LionFire.Services.Services
{
    public class SleepService : IDisposable
    {
        public SleepService()
        {
            for (int i = 5; i >= 0; i--)
            {
                Console.WriteLine("Time left: " + i);
                Thread.Sleep(1000);
                if (isDisposed)
                {
                    Console.WriteLine("SleepService: Disposed.");
                }
            }
        }

        bool isDisposed = false;

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}
