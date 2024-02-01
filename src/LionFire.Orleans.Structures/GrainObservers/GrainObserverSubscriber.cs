using DynamicData;
using LionFire.Orleans_.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Orleans_.ObserverGrains;

public class GrainObserverSubscriber<T>(IGrainObservableAsyncObservableG<T> observableGrain)
{
    public GrainObserverStatus Status { get; } = new();

}
