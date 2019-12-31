using LionFire.MultiTyping;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Internals
{
    public static class IVobInternalsMultiTypingExtensions
    {
        public static IMultiTyped MultiTyped(this IVob vob) => (vob as IVobInternals).MultiTyped;
    }
}
