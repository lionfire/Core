using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public interface IFreezable
    {
        bool IsFrozen { get; set; }
    }

    public static class FreezableExtensions
    {
        public static void Freeze(this IFreezable freezable)
        {
            freezable.IsFrozen = true;
        }
    }
}
