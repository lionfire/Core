using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IHasRunTask
    {
        Task RunTask { get; }
    }

}
