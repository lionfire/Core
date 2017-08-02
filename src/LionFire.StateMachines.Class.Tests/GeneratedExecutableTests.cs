using Xunit;
using LionFire.Execution;
using ExecutionState = LionFire.Execution.ExecutionState2;

namespace LionFire.StateMachines.Class.Tests
{
    public class GeneratedExecutableTests
    {

        public GeneratedExecutableTests()
        {
        }

        [Fact]
        public void Current_State_Flow()
        {
            var te = new GeneratedExecutable();
            Assert.Equal(te.StateMachine.CurrentState, ExecutionState.Uninitialized);
            te.Initialize();
            Assert.Equal(te.StateMachine.CurrentState, ExecutionState.Ready);
            te.Start();
            Assert.Equal(te.StateMachine.CurrentState, ExecutionState.Running);
            te.Complete();
            Assert.Equal(te.StateMachine.CurrentState, ExecutionState.Finished);
            //te.Dispose();
            //Assert.Equal(te.StateMachine.CurrentState, ExecutionState.Disposed);
        }

        [Fact]
        public void OnStateHandlers()
        {
            var te = new GeneratedExecutable();

            te.Initialize();
            Assert.Equal(te.LastMessage.Pop(), "OnReady");
            //Assert.Equal(te.LastMessage.Pop(), "OnInitialize");
            //Assert.Equal(te.LastMessage.Pop(), "AfterUninitialized");
            te.Start();
            //Assert.Equal(te.LastMessage.Pop(), "OnStart");
            //Assert.Equal(te.LastMessage.Pop(), "AfterReady");
            te.Complete();
            //Assert.Equal(te.LastMessage.Pop(), "AfterComplete");
            //te.Dispose();
        }
    }
}
