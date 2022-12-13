using ICSharpCode.SharpZipLib.Zip;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.SharpZipLib_;

public class ReadHandleProvider<TReferenceValue, THandle> : IReadHandleProvider<IReference<TReferenceValue>>
    where THandle : IReadHandle<TReferenceValue>
{
    public IServiceProvider ServiceProvider { get; }

    public ReadHandleProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IReadHandle<TValue> GetReadHandle<TValue>(IReference<TReferenceValue> reference)
    {
        if (typeof(TReferenceValue) != typeof(TValue)) throw new ArgumentException($"Only {typeof(TReferenceValue).FullName} is supported for {nameof(TValue)}");

        return (IReadHandle<TValue>)ActivatorUtilities.CreateInstance<THandle>(ServiceProvider, reference);
    }
}


public class SharpZipLibHandleProvider : ReadHandleProvider<ZipFile, RZip>
{
    public SharpZipLibHandleProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
