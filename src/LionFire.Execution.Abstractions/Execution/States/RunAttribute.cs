using System;

namespace LionFire.Execution
{
    // FUTURE: Simply have a Run() method, or Launch() to run and then forget
    // 
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class RunAttribute : Attribute
    {
        public RunAttribute() { }
    }
}
