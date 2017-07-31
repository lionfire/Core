using System.Collections.Generic;
using System;
using LionFire.Execution;

namespace LionFire.StateMachines.Tests
{
    //public partial class GeneratedExecutable
    //{

    //     //public void Initialize() => StateMachine.ChangeState(ExecutionTransition.Initialize);
    //    public void Initialize() => StateMachine.ChangeState(Transitions.Initialize);
    //    public void Start() => StateMachine.ChangeState(Transitions.Start);
    //    //public void Complete() => StateMachine.ChangeState(ExecutionTransition.Complete);
    //    //public void Terminate() => StateMachine.ChangeState(ExecutionTransition.Terminate);
    //    //public void Dispose() => StateMachine.ChangeState(ExecutionTransition.Dispose);

    //    public static class Transitions
    //    {
    //        public static StateTransitionTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Initialize = new StateTransitionTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionTransition.Initialize)
    //        {
    //            Info = StateMachine<ExecutionState, ExecutionTransition>.GetTransitionInfo(ExecutionTransition.Initialize),
    //            OnTransitioningMethod = (owner) => owner.OnInitializing(),
    //            From = States.Uninitialized,
    //            To = States.Ready,
    //        };
    //        public static StateTransitionTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Start = new StateTransitionTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionTransition.Initialize)
    //        {
    //            Info = StateMachine<ExecutionState, ExecutionTransition>.GetTransitionInfo(ExecutionTransition.Start),
    //            OnTransitioningMethod = (owner) => owner.OnStarting(),
    //            From = States.Ready,
    //            To = States.Running,
    //        };
    //    }
    //    public static class States
    //    {
    //        public static StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Uninitialized = new StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionState.Uninitialized)
    //        {
    //            //EnteringStateAction = owner => owner.OnReady(),
    //            OnLeaving = owner => owner.AfterUninitialized(),
    //        };
    //        public static StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Ready = new StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionState.Ready)
    //        {
    //            OnEntering = owner => owner.OnReady(),
    //            OnLeaving = owner => owner.AfterReady(),
    //        };
    //        public static StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Running = new StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionState.Running)
    //        {
    //            //EnteringStateAction = owner => owner.OnReady(),
    //            //LeavingStateAction = owner => owner.AfterReady(),
    //        };
    //        public static StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable> Finished = new StateTypeBinding<ExecutionState, ExecutionTransition, GeneratedExecutable>(ExecutionState.Finished)
    //        {
    //            OnEntering = owner => owner.OnFinished(),
    //            //LeavingStateAction = owner => owner.AfterReady(),
    //        };
    //    }
    //    public StateMachineState<ExecutionState, ExecutionTransition, GeneratedExecutable> StateMachine => stateMachine;
    //    private StateMachineState<ExecutionState, ExecutionTransition, GeneratedExecutable> stateMachine;
    //}



    [StateMachine(StateMachineOptions.StateMethods
    //, ExecutionTransition.Initialize | ExecutionTransition.Start | ExecutionTransition.Finish | ExecutionTransition.CleanUp
    )]
    public partial class GeneratedExecutable
    {

        #region To generate

        private void InitStateMachine()
        {
            stateMachine = StateMachine<TState, ExecutionTransition>.CreateState(this);
        }

        StateMachineState<TState, ExecutionTransition,GeneratedExecutable> stateMachine;

        #endregion

        public GeneratedExecutable()
        {
            InitStateMachine(); // To generate?
            Action initMethod = Initialize;
        }

        public void AfterUninitialized()
        {
            Log("AfterUninitialized");
        }
        public void OnInitialize()
        {
            Log("OnInitializing");
        }
        public void OnReady()
        {
            Log("OnReady");
        }
        public void AfterReady()
        {
            Log("AfterReady");
        }
        public void OnStarting()
        {
            Log("OnStarting");
        }
        public void AfterComplete()
        {
            Log("AfterComplete");
        }

        public void OnFinished()
        {
            Log("OnFinished");
        }

        public Stack<string> LastMessage { get; set; } = new Stack<string>();

        private void Log(string msg) => LastMessage.Push(msg);
    }
}
