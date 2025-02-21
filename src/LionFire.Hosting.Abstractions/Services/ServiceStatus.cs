using DynamicData;
//using LionFire.Data.Async;
using LionFire.Structures;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting.Services;

public partial class ServiceStatus : ReactiveObject, IKeyed<string>
{
    #region Lifecycle

    public ServiceStatus(object obj, string? key = null) : this(key ?? obj.GetType().FullName!)
    {
        Object = obj;
    }

    public ServiceStatus(string key)
    {
        Key = key;
    }

    #endregion

    public string Key { get; set; }

    [Reactive]
    private object? _object;

    [Reactive]
    private string? _status;


    public SourceList<Exception> Faults { get; } = new SourceList<Exception>();
    public SourceList<Exception> Errors { get; } = new SourceList<Exception>();
    public void ClearErrors() => Errors.Clear();

    [Reactive]
    private ServiceState _state;

    public void OnStarting()
    {
        Faults.Clear();
        State = ServiceState.Starting;
    }

    public void OnRunning() => State = ServiceState.Running;
    public void OnStopping() => State = ServiceState.Stopping;
    public void OnStopped() { State = ServiceState.Stopped; Faults.Clear(); }

    public void OnFaulted(Exception ex)
    {
        Faults.Add(ex);
    }

    public void OnError(Exception ex)
    {
        Errors.Add(ex);
    }

    [Reactive]
    private bool _isHealthy;

    [Reactive]
    private bool _isRestarting;
}
