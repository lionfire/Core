using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{

    public static class IReferenceValidationExtensions
    {
        public static bool IsValid(this IReference reference) // move to IReference as virtual method
        {
            return true; // TODO
            //return OBus.IsValid(reference);
        }
    }
}
