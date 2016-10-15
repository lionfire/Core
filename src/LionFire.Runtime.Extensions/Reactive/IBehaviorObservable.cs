using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Reactive
{
    public interface IBehaviorObservable<T> : IObservable<T>
    {
        T Value { get; }
    }
}
