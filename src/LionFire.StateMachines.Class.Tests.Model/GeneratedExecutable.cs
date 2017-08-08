using System.Collections.Generic;
using System;
using LionFire.Execution;
using TS = LionFire.Execution.ExecutionState2;

namespace LionFire.StateMachines.Class.Tests
{
    [StateMachine(typeof(TS), typeof(ExecutionTransition), GenerateStateMachineFlags.None, IncludeStates = TS.Disposed)]
    //StateMachineStatePropertyName ="sm",
    //, ExecutionTransition.Initialize | ExecutionTransition.Start | ExecutionTransition.Finish | ExecutionTransition.CleanUp
    public partial class GeneratedExecutable
    {

        #region To generate

        private void InitStateMachine()
        {
            stateMachine = StateMachine<TS, ExecutionTransition>.CreateState(this);
        }

        public StateMachineState<TS, ExecutionTransition, GeneratedExecutable> StateMachine => stateMachine;
        StateMachineState<TS, ExecutionTransition,GeneratedExecutable> stateMachine;

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
            Log("OnInitialize");
        }
        public void OnReady()
        {
            Log("OnReady");
        }
        public void AfterReady()
        {
            Log("AfterReady");
        }
        public void OnStart()
        {
            Log("OnStart");
        }
        public void AfterComplete()
        {
            Log("AfterComplete");
        }

        public void OnFinished()
        {
            Log("OnFinished");
        }

        public Queue<string> LogMessageQueue { get; set; } = new Queue<string>();

        private void Log(string msg) => LogMessageQueue.Enqueue(msg);
    }

    //public partial class GeneratedExecutable
    //{
    //    public static class Transitions
    //    {
    //        public static StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Initialize = new StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(ExecutionTransition.Initialize)
    //        {
    //            Info = StateMachine<TS, ExecutionTransition>.GetTransitionInfo(ExecutionTransition.Initialize),
    //            OnTransitioningMethod = (owner) => owner.OnInitializing(),
    //            From = States.Uninitialized,
    //            To = States.Ready,
    //        };
    //        public static StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Start = new StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(ExecutionTransition.Initialize)
    //        {
    //            Info = StateMachine<TS, ExecutionTransition>.GetTransitionInfo(ExecutionTransition.Start),
    //            OnTransitioningMethod = (owner) => owner.OnStarting(),
    //            From = States.Ready,
    //            To = States.Running,
    //        };
    //    }
    //    public static class States
    //    {
    //        public static StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Uninitialized = new StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(TS.Uninitialized)
    //        {
    //            //EnteringStateAction = owner => owner.OnReady(),
    //            OnLeaving = owner => owner.AfterUninitialized(),
    //        };
    //        public static StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Ready = new StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(TS.Ready)
    //        {
    //            OnEntering = owner => owner.OnReady(),
    //            OnLeaving = owner => owner.AfterReady(),
    //        };
    //        public static StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Running = new StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(TS.Running)
    //        {
    //            //EnteringStateAction = owner => owner.OnReady(),
    //            //LeavingStateAction = owner => owner.AfterReady(),
    //        };
    //        public static StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Finished = new StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(TS.Finished)
    //        {
    //            OnEntering = owner => owner.OnFinished(),
    //            //LeavingStateAction = owner => owner.AfterReady(),
    //        };
    //    }
    //    public StateMachineState<TS, ExecutionTransition, GeneratedExecutable> StateMachine => stateMachine;
    //    private StateMachineState<TS, ExecutionTransition, GeneratedExecutable> stateMachine;
    //}

}
