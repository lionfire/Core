using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface IHandleProvider
    {
        H<T> ToHandle<T>(IReference reference, bool throwOnFail = false) where T : class;
    }
}
