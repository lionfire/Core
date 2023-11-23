using System.Collections.Generic;
using LionFire.Structures;
using LionFire.Serialization;
using System.IO;
using LionFire.ExtensionMethods;
using LionFire.Persistence.Handles;
using System.ComponentModel;
using LionFire.Assets;
using System.Threading.Tasks;
using LionFire.Persistence.Assets;
using LionFire.Execution;
//using LionFire.Execution.Executables;
using LionFire.StateMachines;
using LionFire.StateMachines.Class;
using LionFire.Persistence;
using LionFire.MultiTyping;
using System.Threading;
using LionFire.Dependencies;

namespace LionFire.Assets;

//public class TLiveAssetCollection<TAsset>
//{
//    public 
//}

public class LiveAssetCollection<TAsset> : ObservableHandleDictionary<string, AssetHandle<TAsset>, TAsset>, INotifyPropertyChanged
    , IStartable // Maybe replace IStartable with pure IExecutable2 later? Or generate the IStartable from CSM.Generation
    , IStoppable
where TAsset : class
{
    #region State Machine

    public IStateMachine<ExecutionState2, ExecutionTransition> StateMachine => stateMachine;
    private IStateMachine<ExecutionState2, ExecutionTransition> stateMachine;

    //IStateMachine<ExecutionState2, ExecutionTransition> IHas<IStateMachine<ExecutionState2, ExecutionTransition>>.Object => stateMachine;

    #endregion

    #region (Static) Instance

    public static LiveAssetCollection<TAsset> Instance => instance;
    private static LiveAssetCollection<TAsset> instance = new LiveAssetCollection<TAsset>();

    #endregion

    #region Construction

    protected LiveAssetCollection()
    {
        stateMachine = StateMachine<ExecutionState2, ExecutionTransition>.Create(this);
        //fsObjects.RootPath = Path.Combine(LionFireEnvironment.AppProgramDataDir, "HeartMonitors"); //  Use asset ifranstructure to set this more automatically
    }

    #endregion

    #region Relationships

    #region AssetProvider

    public IAssetProvider AssetProvider
    {
        get { if (assetProvider == null) { assetProvider = DependencyContext.Current.GetService<IAssetProvider>(); } return assetProvider; }
    }
    private IAssetProvider assetProvider;

    public INotifyingAssetProvider<TAsset> NotifyingAssetProvider => AssetProvider as INotifyingAssetProvider<TAsset>;

    #endregion

    #endregion

    #region State Machine

    List<string> assetDirectories = new List<string>();
    /// <summary>
    /// FUTURE: 
    /// </summary>
    public bool WatchRecursively { get; set; }
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (NotifyingAssetProvider != null)
        {
            NotifyingAssetProvider.ListeningToPersistenceEvents = true;
        }
        await RefreshHandles();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (NotifyingAssetProvider != null)
        {
            NotifyingAssetProvider.ListeningToPersistenceEvents = false;
        }
        return Task.CompletedTask;
    }

    #endregion

#region Methods

#if TODO
    public override async Task RefreshHandles()
    {
        handles.SetToMatch((await AssetProvider.FindHandles<TAsset>()).ToDictionary<string, AssetHandle<TAsset>>());
    }
#endif
    
#endregion

#region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

#endregion

}
