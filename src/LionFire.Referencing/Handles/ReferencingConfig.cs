using LionFire.Referencing.Persistence;
using System;

namespace LionFire.Referencing
{
    public static class ReferencingConfig
    {
        public static Func<IReferenceRetriever> DefaultRetriever { get; set; }
    }
}
