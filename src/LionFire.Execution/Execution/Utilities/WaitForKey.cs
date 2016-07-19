using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution.Utilities
{
    public class TestService
    {
        public TestService()
        {
            for (int i = 5; i >= 0; i--)
            {
                Console.WriteLine("Time left: " + i);
                Thread.Sleep(1000);
            }
        }
    }

    public class WaitForKey
    {
        public WaitForKey()
        {
            //Console.WriteLine();
            Console.ReadKey();
        }
    }

}
