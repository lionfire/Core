using System;

namespace LionFire.Structures
{
    public interface IHasLastException
    {
        Exception LastException { get; }
    }
    
}
