using System;

namespace LionFire.Types
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class InterfaceKindAttribute : Attribute
    {
        public InterfaceKind InterfaceKind { get; private set; }
        public InterfaceKindAttribute(InterfaceKind interfaceKind)
        {
            this.InterfaceKind = interfaceKind;
        }
    }
}
