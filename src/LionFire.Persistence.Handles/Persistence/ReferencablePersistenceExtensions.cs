using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Persistence
{
    public static class ReferencableSaveExtensions
    {
        public static async Task<IPersistenceResult> Save<T>(this T referencable)
            where T : IReferencable
        {
            var handle = referencable.Reference.GetReadWriteHandle<T>();
            handle.Value = referencable;
            return (IPersistenceResult)await handle.Put().ConfigureAwait(false);
        }
        public static async Task<T> Saved<T>(this T referencable)
           where T : IReferencable
        {
            await referencable.Save().ConfigureAwait(false);
            return referencable;
        }
    }
}
