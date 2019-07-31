using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public static class IReferenceableExtensions
    {
        
        public static string Name(this IReferencable referencable) => referencable.Reference.Name();
    }
}
