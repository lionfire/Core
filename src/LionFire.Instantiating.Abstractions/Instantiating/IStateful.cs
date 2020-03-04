using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Instantiating
{
    public interface IStateful<TState> : IStateful
    {
        new TState State { get; set; }
    }

    public interface IStateful
    {
        object State { get; set; }
    }
}
