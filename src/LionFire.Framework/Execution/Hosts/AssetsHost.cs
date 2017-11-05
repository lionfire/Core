using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Collections;
using LionFire.Execution.Executables;
using LionFire.Instantiating;
using LionFire.StateMachines.Class;
using LionFire.Validation;

namespace LionFire.Execution.Hosts
{

    //public class TestExecutable : ExecutableBase
    //{
    //    private void StateMachine_StateChanging(StateMachines.IStateChange<ExecutionState2, ExecutionTransition> context)
    //    {
    //        //Debug.WriteLine(StateMachine_StateChanging);
    //    }
    //}

    //public class ExecutableHost : ExecutableBase, IExecutable2
    //{

    //    public IExecutable2 Child
    //    {
    //        get
    //        {
    //            return child;
    //        }
    //        set
    //        {
    //            if (child == value) return;
    //            if (StateMachine.CurrentState != ExecutionState2.Uninitialized) throw new InvalidExecutionStateException("Set child", ExecutionStateEx.Uninitialized, StateMachine.CurrentState);
    //            child = value;
    //        }
    //    }
    //    private IExecutable2 child;

    //    public Func<ValidationContext> vc => () => new ValidationContext(this);

    //    public object CanInitialize
    //    {
    //        get {
    //            var v = Validate.For(this).PropertyNotNull(nameof(Child), Child))); if (!v.Valid) return v.Issues;
    //            var result = Child.CanInitialize
    //        }

    //        return null;
    //    }

    //}

    public class ExecutionGoal
    {
        public IExecutable2 Executable { get; set; }
        public ExecutionState2 DesiredState { get; set; }
        public List<object> FailureReasons { get; set; }

        public bool IsFaulted => FailureReasons != null && FailureReasons.Any();
    }

    

    public class ExecutablesHost : ExecutableBase, ICollection<IExecutable2>
    {
        #region Construction

        public ExecutablesHost()
        {
            faultedExecutablesRO = new ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal>(faultedExecutables);
            transitioningExecutablesRO = new ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal>(transitioningExecutables);
        }

        #endregion

        #region Collections

        ObservableCollection<IExecutable2> hostedExecutables = new ObservableCollection<IExecutable2>();

        public IReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> FaultedExecutables
        {
            get
            {
                return faultedExecutablesRO;
            }
        }
        ObservableDictionary<IExecutable2, ExecutionGoal> faultedExecutables = new ObservableDictionary<IExecutable2, ExecutionGoal>();
        ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> faultedExecutablesRO;

        public IReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> TransitioningExecutables
        {
            get
            {
                return transitioningExecutablesRO;
            }
        }

        ObservableDictionary<IExecutable2, ExecutionGoal> transitioningExecutables = new ObservableDictionary<IExecutable2, ExecutionGoal>();
        ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> transitioningExecutablesRO;

        #endregion

        #region (Protected) Event Handlers

        protected virtual void OnAdded(IExecutable2 exe)
        {
            if (exe.StateMachine().CurrentState != this.StateMachine().CurrentState)
            {
                transitioningExecutables.Add(exe, new ExecutionGoal { Executable = exe, DesiredState = this.StateMachine().CurrentState });
                exe.TransitionToState(this.StateMachine().CurrentState);
            }
        }

        #endregion

        #region ICollection Implementation

        public int Count => hostedExecutables.Count;

        public bool IsReadOnly => false;

        public void Add(IExecutable2 exe)
        {
            try
            {
                hostedExecutables.Add(exe);
            }
            catch
            {
                Remove(exe);
                throw;
            }
        }

        public bool Remove(IExecutable2 exe)
        {
            var r1 = hostedExecutables.Remove(exe);
            var r2 = faultedExecutables.Remove(exe);
            var r3 = transitioningExecutables.Remove(exe);
            return r1 || r2 || r3;
        }

        public void Clear()
        {
            hostedExecutables.Clear();
            faultedExecutables.Clear();
            transitioningExecutables.Clear();
        }

        public bool Contains(IExecutable2 item)
        {
            return hostedExecutables.Contains(item);
        }

        public void CopyTo(IExecutable2[] array, int arrayIndex)
        {
            hostedExecutables.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IExecutable2> GetEnumerator()
        {
            return hostedExecutables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Automatically instantiates all assets of Type TAsset and starts them.
    /// TODO:
    ///  - Stop on delete
    ///  - Restart/signal on change.  Interface for this?  OnTemplateChanged(TAsset )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[GenerateStateMachine(Async=true)] // TODO: Rename to GenerateStateMachine and don't require types if there is already a StateMachineState property with specified types
    //
    public class AssetsHost<TAsset> : ExecutablesHost, IStartable, IInitializable3
        where TAsset : class
    {
        LiveAssetCollection<TAsset> assets;
        public IReadOnlyCollectionAdapter<object> Instances => instances;
        IReadOnlyCollectionAdapter<object> instances;

        Dictionary<TAsset,object> hostedInstances = new Dictionary<TAsset, object>();

        public void OnInitialize()
        {
            assets = LiveAssetCollection<TAsset>.Instance;

            //FUTURE: how to map LiveAssetCollection<> or some collection with Handles to this instances collection?  Should there be some addable collection that is transparently handled by a live object/asset collection?
            // TODO: SaveManager.CanSaveType<T>()?  SaveManager.Save<T>(obj);  SaveContext.Add(myCollection);  SaveContext.AutoSave = true;  SaveContext.Description = "local auto-save objects"
            IEnumerable<TAsset> e = assets.Objects;

            instances = e.GetReadOnlyInstancesAdapter<TAsset, object>();
            instances.CollectionChanged += Instances_CollectionChanged;
            Debug.WriteLine("OnInitialized! " + assets.Handles.Count + " assets, " + instances.Count + " objects");
            assets.Start();
        }

        private void Instances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        Debug.WriteLine("New item: " + item);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        Debug.WriteLine("Old item: " + item);
                    }
                }
            }
        }

        public void OnReady()
        {
            Debug.WriteLine("OnReady");
        }
        public void OnStart()
        {
            Debug.WriteLine("OnStart");
            //assets.Handles.Added += Handles_Added;
            //assets.Handles.Removed += Handles_Removed;
        }

        protected void OnAdded(TAsset newAsset)
        {
            object newItem = newAsset;
            var template = newAsset as ITemplate;
            while (template != null)
            {
                newItem = template.Create();
            }
            hostedInstances.Add(newAsset, newItem);
        }

        //private void Handles_Removed(string arg1, Persistence.Assets.AssetHandle<TAsset> arg2)
        //{
        //}

        //private void Handles_Added(string arg1, Persistence.Assets.AssetHandle<TAsset> arg2)
        //{
        //    var obj = arg2.Object;
        //    if (obj == null) return;
        //    if (obj is ITemplate)
        //    {
        //    }
        //}

        private void SyncHostedObjects()
        {
            //hostedObjects.SetToMatch(assets.Handles, )
        }



        private void Test()
        {
            Debug.WriteLine("Test");
            Initialize();
        }

        #region TODO: Generate this once generation works

        public Task<object> Initialize()
        {
            StateMachine.Transition(ExecutionTransition.Initialize);
            return Task.FromResult<object>(null);
        }
        //public void InitializeX() => StateMachine.ChangeState(ExecutionTransition.Initialize);
        public Task Start() { StateMachine.Transition(ExecutionTransition.Start); return Task.CompletedTask; }
        public void Complete() => StateMachine.Transition(ExecutionTransition.Complete);

        #endregion

    }
}
