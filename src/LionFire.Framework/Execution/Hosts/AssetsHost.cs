using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Execution.Executables;
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
    //[GenerateStateMachine] // TODO: Rename to GenerateStateMachine and don't require types if there is already a StateMachineState property with specified types
    public class AssetsHost<TAsset> : ExecutableBase, IStartable, IInitializable2
        where TAsset : class
    {

        LiveAssetCollection<TAsset> assets = LiveAssetCollection<TAsset>.Instance;

        public void OnInitialize()
        {
            throw new System.Exception("OnInitialized!");
        }
        public void OnStart()
        {
            throw new System.Exception("started!");
        }

        private void Test()
        {
            Initialize();
        }


        #region TODO: Generate this once generation works

        public Task<ValidationContext> Initialize()
        {
            StateMachine.Transition(ExecutionTransition.Initialize);
            return Task.FromResult<ValidationContext>(null);
        }
        //public void InitializeX() => StateMachine.ChangeState(ExecutionTransition.Initialize);
        public Task Start() { StateMachine.Transition(ExecutionTransition.Start); return Task.CompletedTask; }
        public void Complete() => StateMachine.Transition(ExecutionTransition.Complete);

        #endregion

    }
}
