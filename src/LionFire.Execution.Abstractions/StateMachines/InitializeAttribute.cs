using System;

namespace LionFire.Execution
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class InitializeAttribute : Attribute
    {
        public InitializeAttribute() { }
    }
}
