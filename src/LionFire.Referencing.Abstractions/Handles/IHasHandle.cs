﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public interface IHasHandle
    {
        IHandle Handle { get;  }
    }
}
