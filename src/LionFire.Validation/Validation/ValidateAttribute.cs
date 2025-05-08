using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Validation
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ValidateAttribute : Attribute
    {
        public ValidateAttribute()
        {
        }
    }


}
