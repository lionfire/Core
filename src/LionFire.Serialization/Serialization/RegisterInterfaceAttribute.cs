using System;

namespace LionFire.Serialization
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RegisterInterfaceAttribute : Attribute // FUTURE MOVE if desired
    {
        public Type[] Types { get; set; }
        public RegisterInterfaceAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }

}
