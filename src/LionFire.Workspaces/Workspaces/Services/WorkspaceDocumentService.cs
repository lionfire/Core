using DynamicData;
using LionFire.Execution;
using LionFire.FlexObjects;
using LionFire.Reactive.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Workspaces.Services;


public abstract class WorkspaceDocumentService<TKey, TValue /*,TValueVM, TRunner*/> : IHostedService
where TKey : notnull
where TValue : notnull
//where TValueVM : IEnablable
//where TRunner : IRunner<TValue> // DEPRECATED - TRunner (use IWorkspaceDocumentRunner<TKey, TValue> instead, which allows 0 or more instead of exactly one)
{

    public IObservableReaderWriter<TKey, TValue> Data { get; }
    public IServiceProvider ServiceProvider { get; }
    public ILogger Logger { get; }
    public IEnumerable<IWorkspaceDocumentRunner<TKey, TValue>> RunnerServices { get; }

    public WorkspaceDocumentService(IServiceProvider serviceProvider, ILogger logger, IObservableReaderWriter<TKey, TValue> data
        )
    {
        ServiceProvider = serviceProvider;

        RunnerServices = ServiceProvider.GetRequiredService<IEnumerable<IWorkspaceDocumentRunner<TKey, TValue>>>();
        if (RunnerServices.Any()) { runners2 = [.. Enumerable.Range(0, RunnerServices.Count()).Select(_ => new ConcurrentDictionary<TKey, IObserver<TValue>>())]; }

        foreach (var runnerService in RunnerServices)
        {
            if (!runnerService.RunnerType.IsAssignableTo(typeof(IObserver<TValue>)))
            {
                throw new ArgumentException($"RunnerType {runnerService.RunnerType} must implement IObserver<{typeof(TValue).FullName}>");
            }
        }

        Logger = logger;
        Data = data;

        Data.Keys.Connect().Subscribe(changeSet =>
        {
            var data = Data;
            foreach (var c in changeSet)
            {
                Logger.LogInformation($"[Keys {c.Reason}] " + c.Current);
            }
        });
        Data.Values.Connect().Subscribe(changeSet =>
        {
            foreach (var change in changeSet)
            {
                Logger.LogInformation($"[Values {change.Reason.ToString()}] {(change.Current.HasValue ? change.Current.Value : null)}");
            }
        });

        Task.Run(async () =>
        {
            disposables?.Add(await Data.ListenAllValues());
        });
    }
    IDisposable? listenAllSubscription;

    CompositeDisposable? disposables = new();
    public Task StartAsync(CancellationToken cancellationToken)
    {
        disposables = new();
        listenAllSubscription = Data.ListenAllKeys();
        disposables.Add(Data.Values.Connect().Subscribe(async v => await OnValue(v)));
        //disposables.Add(Data.ObservableCache.Connect().Subscribe(v => OnValue(v)));

        foreach (var key in Data.Keys.Keys)
        {
            Logger.LogInformation("StartAsync.  key: {0}", key);
        }
        return Task.CompletedTask;
    }

    //ConcurrentDictionary<TKey, TRunner> runners = new();
    List<ConcurrentDictionary<TKey, IObserver<TValue>>> runners2 = new();

    private async ValueTask OnValue(DynamicData.IChangeSet<DynamicData.Kernel.Optional<TValue>, TKey> changeSet)
    {
        List<Task>? tasks = null;
        foreach (var change in changeSet)
        {
            if (change.Current.HasValue)
            {
                //Logger.LogInformation("Got value: {0}", change.Key);
                onValue(change.Key, change.Current.Value);
            }
            else
            {
                Logger.LogInformation("No value yet, but it should arrive soon: {0}", change.Key);
                //(tasks ??= []).Add(Task.Run(async () =>
                //{
                //    var result = await Data.TryGetValue(change.Key);
                //    if (result.HasValue)
                //    {
                //        onValue(change.Key, result.Value);
                //    }
                //}));
            }

            void onValue(TKey k, TValue v)
            {
                foreach (var runnerService in RunnerServices.Select((value, i) => new { i, value }))
                {
                    var runner = runners2[runnerService.i].GetOrAdd(k, _ => (IObserver<TValue>)ActivatorUtilities.CreateInstance(ServiceProvider, runnerService.value.RunnerType));
                    Logger.LogInformation($"onValue: {change.Reason.ToString()} {(change.Current.HasValue ? change.Current.Value : null)}");
                    runner.OnNext(v);
                }

                // OLD
                //{
                //    var runner = runners.GetOrAdd(k, _ => ActivatorUtilities.CreateInstance<TRunner>(ServiceProvider));
                //    Logger.LogInformation($"onValue: {change.Reason.ToString()} {(change.Current.HasValue ? change.Current.Value : null)}");
                //    runner.OnNext(v);
                //}
            }
        }
        if (tasks != null) await Task.WhenAll(tasks);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        disposables?.Dispose();
        disposables = null;
        listenAllSubscription?.Dispose();
        return Task.CompletedTask;
    }
}
