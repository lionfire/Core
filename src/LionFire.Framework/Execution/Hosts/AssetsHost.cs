using System.Diagnostics;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Execution.Executables;
using LionFire.StateMachines.Class;
using LionFire.Validation;

namespace LionFire.Execution.Hosts
{

    public class TestExecutable : ExecutableBase
    {
        private void StateMachine_StateChanging(StateMachines.IStateChange<ExecutionState2, ExecutionTransition> context)
        {
            //Debug.WriteLine(StateMachine_StateChanging);
        }
    }

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

        LiveAssetCollection<TAsset> assets;

     

        private void StateMachine_StateChanged(StateMachines.IStateChange<ExecutionState2, ExecutionTransition> context)
        {
            Debug.WriteLine("StateMachine_StateChanged");
        }

        public void OnInitialize()
        {
            assets = LiveAssetCollection<TAsset>.Instance;
            Debug.WriteLine("OnInitialized!");
        }

        public void OnReady()
        {
            Debug.WriteLine("OnReady");
        }
        public void OnStart()
        {
            Debug.WriteLine("OnStart");
        }

        private void Test()
        {
            Debug.WriteLine("Test");
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
