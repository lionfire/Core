﻿using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Data;
using LionFire.Data.Async.Sets;

namespace LionFire.ExtensionMethods.Persistence; // REVIEW - change namespace to LionFire.ExtensionMethods.Data?

public static class ReferenceableSaveExtensions
{        
    public static async Task<ITransferResult> Save<T>(this T referencable)
      where T : IReferenceable
    {
        if(referencable is IReadWriteHandle rwh)
        {
            // UNTESTED
            return (ITransferResult)await rwh.Set().ConfigureAwait(false);
        }

        var result = await TrySave(referencable).ConfigureAwait(false);
        if (result.IsSuccess() != true)
        {
            throw new TransferException(result, $"Save of '{referencable.Reference}' failed: {result}");
        }
        return result;
    }

    public static async Task<ITransferResult> TrySave<T>(this T referencable)
        where T : IReferenceable
    {
        if (typeof(T).IsInterface)
        {
            return await ((Task<ITransferResult>)typeof(ReferenceableSaveExtensions).GetMethod(nameof(TrySave)).MakeGenericMethod(referencable.GetType()).Invoke(null, new object[] { referencable })).ConfigureAwait(false);
        }

        ISetter puts = referencable.GetExistingReadWriteHandle();
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
            var handle2 = referencable.Reference.GetReadWriteHandlePrestaged(type, initialValue, overwriteValue: true).handle;
            puts = handle2;
        }
        
        return (ITransferResult)await puts.Set().ConfigureAwait(false);
    }

    public static async Task<T> Saved<T>(this T referencable)
       where T : IReferenceable
    {
        await referencable.Save().ConfigureAwait(false);
        return referencable;
    }
}
