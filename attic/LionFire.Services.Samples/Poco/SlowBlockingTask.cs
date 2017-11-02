
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services.Samples
{
    public class SlowBlockingTask
    {
        public SlowBlockingTask()
        {
            Console.WriteLine($" --- {this.GetType().Name} starting --- ");
            Console.Write("Performing slow task...");
            Thread.Sleep(10 * 1000);
            Console.Write("done.");
            Console.WriteLine($" --- {this.GetType().Name} finished --- ");
        }
    }
}
