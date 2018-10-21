using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public class DeferredResolutionHandleProvider : IHandleProvider
    {
        public H<T> ToHandle<T>(IReference reference)
            where T : class
            => new HDynamic<T>(reference);
            
    }
}
