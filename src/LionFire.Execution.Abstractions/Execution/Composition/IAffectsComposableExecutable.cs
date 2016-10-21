using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Composition
{
    public enum ComposableStates
    {
        Configuration,
        Initialization,
        Shutdown,
    }

    //public interface IAffectsComposableExecutable
    //{
    //    ComposableStates AffectsComposableState { get; }

    //}
}
