using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public class NoopDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
