using LionFire.Referencing.Resolution;
using System;

namespace LionFire.Referencing
{
    public static class ReferencingConfig
    {
        public static Func<IHandleResolver> DefaultReferenceResolver { get; set; }
    }
}
