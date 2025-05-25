using LionFire.Reactive.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Mvvm;

public class ObservableDataViewVM<TKey, TValue> : ReactiveObject
where TValue : notnull
{
    public IServiceProvider? ServiceProvider { get; set; }

    //    public IObservableReader<string, TValue>? Entities { get; set; }
    //    //public IObservableReaderWriter<string, TValue>? WritableEntities { get; set; }

    //    //public IObservableReaderWriter<string, TValueVM>? EntityVMs { get; set; }

    //public void Init(IServiceProvider? serviceProvider)
    //{
    //    Data =
    //        serviceProvider?.GetService<IObservableReaderWriter<string, TValue>>()
    //        ?? serviceProvider?.GetService<IObservableReader<string, TValue>>();
    //}

    //public IObservableReader<string, TValue>? Data { get; set; }

}
