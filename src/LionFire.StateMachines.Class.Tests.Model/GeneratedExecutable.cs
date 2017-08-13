using System.Collections.Generic;
using System;
using LionFire.Execution;
using TS = LionFire.Execution.ExecutionState2;

namespace LionFire.StateMachines.Class.Tests
{
    [StateMachine(typeof(ExecutionState2), typeof(ExecutionTransition), GenerateStateMachineFlags.None, IncludeTransitions = ExecutionTransition.Dispose
        //, IncludeStates = ExecutionState2.Disposed
        )]
    //StateMachineStatePropertyName ="sm",
    //, ExecutionTransition.Initialize | ExecutionTransition.Start | ExecutionTransition.Finish | ExecutionTransition.CleanUp
    public partial class GeneratedExecutable
    {
        public void AfterUninitialized()
        {
            Log("AfterUninitialized");
        }

        public bool InitializePrereq { get; set; }
        public int CanInitializeCount { get; set; }
        public bool CanInitialize
        {
            get
            {
                CanInitializeCount++;
                return InitializePrereq;
            }
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

        #region Log for testing

        public Queue<string> LogMessageQueue { get; set; } = new Queue<string>();

        private void Log(string msg) => LogMessageQueue.Enqueue(msg);

        #endregion
    }

    //    public static class Transitions
    //    {
    //        public static StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Initialize = new StateTransitionTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(ExecutionTransition.Initialize)
    //        {
    //            Info = StateMachine<TS, ExecutionTransition>.GetTransitionInfo(ExecutionTransition.Initialize),
    //            OnTransitioningMethod = (owner) => owner.OnInitializing(),
    //            From = States.Uninitialized,
    //            To = States.Ready,
    //        };

    //    public static class States
    //    {
    //        public static StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable> Uninitialized = new StateTypeBinding<TS, ExecutionTransition, GeneratedExecutable>(TS.Uninitialized)
    //        {
    //            //EnteringStateAction = owner => owner.OnReady(),
    //            OnLeaving = owner => owner.AfterUninitialized(),
    //        };



}
