using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    public class DisposeAction : IDisposable
    {
        private Action OnDispose;
        public DisposeAction(Action onDispose) { this.OnDispose = onDispose; }

        public void Dispose()
        {
            OnDispose();
        }
    }
}
