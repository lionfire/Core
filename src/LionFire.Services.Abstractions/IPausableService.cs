using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public interface IPausableService
    {
        Task Pause();
        Task Continue();
    }
   
}
