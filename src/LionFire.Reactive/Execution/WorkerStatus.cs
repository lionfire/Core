using LionFire.FlexObjects;
using ReactiveUI;
using Reactive = ReactiveUI.SourceGenerators.ReactiveAttribute;

namespace LionFire.Execution;

public interface IWorkerStatus
{
    RunnerState RunnerState { get; }
}

/// <summary>
/// REVIEW 
/// </summary>
public partial class WorkerStatus : ReactiveObject, IFlex, IWorkerStatus
{
    object? IFlex.FlexData { get; set; }

    #region State

    [ReactiveUI.SourceGenerators.Reactive]
    private RunnerState _runnerState = RunnerState.Unspecified;

    public IObservableCache<(string key, object value), string>? Faults => faults; // ENH: hide key on TObject
    public SourceCache<(string key, object value), string> faults = new(x => x.key);

    #endregion
}
