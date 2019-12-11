#if ASSETS // TODO: enable once Assets is ready to be added back in
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Collections;
using LionFire.Instantiating;
using LionFire.StateMachines.Class;
using LionFire.Validation;

namespace LionFire.Execution.Hosts
{

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

#endif