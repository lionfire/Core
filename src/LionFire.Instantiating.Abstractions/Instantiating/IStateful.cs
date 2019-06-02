using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Instantiating
{
    public interface IStateful<StateType> : IStateful
    {
        new StateType State { get; set; }
    }

    public interface IStateful
    {
        object State { get; set; }
    }


}
