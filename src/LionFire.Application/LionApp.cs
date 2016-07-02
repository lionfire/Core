using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Application
{
    public class LionApp
    {
        public LionApp()
        {
        }



        public virtual void Run()
        {
            Console.WriteLine("LionApp running.  Press any key to exit.  Override LionApp.Run to override this behaviour.");
            Console.ReadKey();
        }
    }
}
