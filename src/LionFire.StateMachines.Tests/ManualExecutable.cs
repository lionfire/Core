using System.Collections.Generic;
using System;
using Xunit;
using LionFire.Execution;

namespace LionFire.StateMachines.Tests
{
    public partial class ManualExecutable
    {

        public void Initialize() => StateMachine.ChangeState(ExecutionTransition.Initialize);
        public void Start() => StateMachine.ChangeState(ExecutionTransition.Start);
        public void Stop() => StateMachine.ChangeState(ExecutionTransition.Stop);
        public void Dispose() => StateMachine.ChangeState(ExecutionTransition.Dispose);


    }

    [StateMachineGeneration(Generate.StateMethods,
        ExecutionTransition.Initialize | ExecutionTransition.Start | ExecutionTransition.Stop | ExecutionTransition.Dispose
        )]
    public partial class ManualExecutable
    {
        public StateMachineState<ExecutionState, ExecutionTransition> StateMachine => stateMachine;
        private StateMachineState<ExecutionState, ExecutionTransition> stateMachine = ExecutableStateModel.Get<ManualExecutable>();

        public void OnInitializing()
        {
            Log("OnInitializing");
        }

        public void AfterReady()
        {
            Log("AfterReady");
        }

        public void AfterStopped()
        {
            Log("AfterStopped");
        }

        public Stack<string> LastMessage { get; set; }

        private void Log(string msg) => LastMessage.Push(msg);
    }

    public class BasicExecutableTests
    {

        public BasicExecutableTests()
        {
        }

        [Fact]
        public void NormalExecutableStateFlow()
        {
            var te = new ManualExecutable();
            Assert.Equal(te.State.CurrentState, ExecutableState.Uninitialized);
            te.Initialize();
            Assert.Equal(te.State.CurrentState, ExecutableState.Ready);
            te.Start();
            Assert.Equal(te.State.CurrentState, ExecutableState.Running);
            te.Stop();
            Assert.Equal(te.State.CurrentState, ExecutableState.Finished);
            Assert.Equal(te.LastMessage.Pop(), "AfterStopped");
            te.Dispose();
            Assert.Equal(te.State.CurrentState, ExecutableState.Disposed);
        }

    }
}
