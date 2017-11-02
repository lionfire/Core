using System;

namespace LionFire.Execution
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class TransitionKindAttribute : Attribute
    {
        public TransitionKindAttribute(TransitionKind kind)
        {
            this.kind = kind;
        }
        public TransitionKind TransitionKind => kind;
        private readonly TransitionKind kind;
    }
}
