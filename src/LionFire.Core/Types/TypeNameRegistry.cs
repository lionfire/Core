using System;
using System.Collections.Generic;

namespace LionFire.Types
{
    public class TypeNameRegistry
    {
        public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
    }
}
