using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Threading
{
    /// <summary>
    /// Marks whether code is thread safe or not
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ThreadSafeAttribute : Attribute
    {
        public bool IsThreadSafe { get; private set; }

        public ThreadSafeAttribute(bool isThreadSafe = true)
        {
            this.IsThreadSafe = isThreadSafe;
        }
    }
}
