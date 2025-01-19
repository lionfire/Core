using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive.Persistence;

public interface IObservableReaderWriter<TKey, TValue> 
    where TKey : notnull
    where TValue : notnull
{
    IObservableReader<TKey, TValue> Read { get; }
    IObservableWriter<TKey, TValue> Write { get; }
}
