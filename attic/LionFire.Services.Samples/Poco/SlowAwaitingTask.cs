using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services.Samples
{
    public class SlowAwaitingTask
    {
        public SlowAwaitingTask()
        {
            Console.WriteLine($" --- {this.GetType().Name} starting --- ");
            Console.Write("Awaiting slow task...");
            System.Threading.Tasks.Task.Delay(10 * 1000).Wait();
            Console.Write("done.");
            Console.WriteLine($" --- {this.GetType().Name} finished --- ");
        }
    }
}
