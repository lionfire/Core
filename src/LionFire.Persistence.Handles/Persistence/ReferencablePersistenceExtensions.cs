using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
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
            if(referencable is IReadWriteHandle rwh)
            {
                // UNTESTED
                return (IPersistenceResult)await rwh.Put().ConfigureAwait(false);
            }

            var result = await TrySave(referencable).ConfigureAwait(false);
            if (result.IsSuccess() != true)
            {
                throw new PersistenceException(result, $"Save of '{referencable.Reference}' failed: {result}");
            }
            return result;
        }

        public static async Task<IPersistenceResult> TrySave<T>(this T referencable)
            where T : IReferencable
        {
            if (typeof(T).IsInterface)
            {
                return await ((Task<IPersistenceResult>)typeof(ReferencableSaveExtensions).GetMethod(nameof(TrySave)).MakeGenericMethod(referencable.GetType()).Invoke(null, new object[] { referencable })).ConfigureAwait(false);
            }

            IPuts puts = referencable.GetExistingReadWriteHandle();
            if (puts == null)
            {
                IReadHandle readHandle = referencable as IReadHandle;

                IReadHandle<object> r = (IReadHandle<object>)referencable.GetExistingReadHandle();

                var value = r.Value; // Potentially BLOCKING

                Type type = (readHandle?.Type) 
                    ?? (referencable.Reference as ITypedReference)?.Type 
                    ?? value?.GetType()
                    ?? throw new ArgumentException($"{nameof(referencable)} must be of type {typeof(ITypedReference).Name} or resolve to a non-default value via IReadHandle<object>.Value.");

                object initialValue;
                if(referencable is IReadWrapper<object> rw)
                {
                    initialValue = rw.Value;
                }
                else if (readHandle != null)
                {
                    initialValue = (T) typeof(IReadWrapper<>).MakeGenericType(readHandle.Type).GetMethod("Value").Invoke(referencable, null);
                }
                else
                {
                    initialValue = referencable;
                }
                var handle2 = referencable.Reference.GetReadWriteHandle(type, initialValue);
                puts = handle2;
            }
            
            return (IPersistenceResult)await puts.Put().ConfigureAwait(false);
        }

        public static async Task<T> Saved<T>(this T referencable)
           where T : IReferencable
        {
            await referencable.Save().ConfigureAwait(false);
            return referencable;
        }
    }
}
