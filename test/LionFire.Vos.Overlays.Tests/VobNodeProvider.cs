using System;

namespace LionFire.Vos.Overlays.Tests
{
    public class VobNodeProvider<T> : IVobNodeProvider
        where T : class
    {
        public VobNode<T> Instance { get; protected set; }

        public Func<IVob, T> Factory { get; set; }
    }
}
