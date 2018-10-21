using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBaseHandleProvider : IHandleProvider
    {
        public FsOBaseHandleProvider(I)
        {

        }
        public H<T> ToHandle<T>(IReference reference, bool throwOnFail = false) 
            where T : class
        {
            new FsOBaseHandle<T>()
            {
                Ref
            }
            
        }
    }
}
