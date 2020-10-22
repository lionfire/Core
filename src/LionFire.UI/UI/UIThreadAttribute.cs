using System;

namespace LionFire.UI
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class UIThreadAttribute : Attribute
    {
        public UIThreadAttribute()
        {
        }
    }
}
