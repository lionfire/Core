using System;

namespace LionFire.Execution
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class RequiredToEnterStateAttribute : Attribute
    {
        public ExecutionStateEx State { get; private set; }
        public RequiredToEnterStateAttribute(ExecutionStateEx state)
        {
            this.State = state;
        }
    }
}