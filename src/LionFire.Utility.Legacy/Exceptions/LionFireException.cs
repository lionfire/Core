﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire;

[Serializable]
public class LionFireException : ApplicationException
{
    public LionFireException() { }
    public LionFireException(string message) : base(message) { }
    public LionFireException(string message, Exception inner) : base(message, inner) { }

}
