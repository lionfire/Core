//using LionFire.StateMachines;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using ES = LionFire.Execution.ExecutionState;
//using ET = LionFire.Execution.ExecutionTransition;

//namespace LionFire.Execution
//{

//    //public class ExecutionStateModel : StateMachine<ES, ET>, IExecutionStateModel
//    //{
//    //}

//    //public interface IExecutionStateModel : IStateModel<ExecutionState, ExecutionTransition> { }
//    public class TExecutableStateMachine
//    {
//        //public static StateMachine<ES, ET> DefaultModel
//        //{
//        //    get
//        //    {
//        //        // TEMP - use enum attributes instead
//        //        var sm = new StateMachine<ES, ET>()
//        //                .Add(ES.Uninitialized, ES.Ready, ET.Initialize)
//        //                //.Add("Uninitialized", "Finished", "Fault.Invalidate")

//        //                //.Add("Ready", "Uninitialized", "Cancel.Deinitialize")
//        //                //.Add("Ready", "Running", "Start")
//        //                //.Add("Ready", "Finished", "Complete.Noop")
//        //                //.Add("Ready", "Finished", "Abort.Skip")

//        //                //.Add("Running", "Ready", "Cancel.Undo")
//        //                //.Add("Running", "Finished", "Complete.Finish")
//        //                //.Add("Running", "Finished", "Abort.Terminate")
//        //                //.Add("Running", "Finished", "Fault.Fail")

//        //                //// Sub states: PauseMachine
//        //                ////.AddTransition("Running", "Paused", "Pause")
//        //                ////.AddTransition("Paused", "Running", "Unpause")

//        //                //// Sub states: ProcessorMachine
//        //                //// Waiting
//        //                //// Processing

//        //                //.Add("Finished", "Ready", "Reset")
//        //                //.Add("Finished", "Uninitialized", "Deinitialize.Reuse")
//        //                //.Add("Uninitialized", "Ready", "Initialize")
//        //                ;
//        //        return sm;
//        //    }
//        //}
//        //public static StateModel<ExecutionState, ExecutionTransition>
//        //public static StateMachineState<ES, ET,TOwner> CreateState<TOwner>(TOwner owner)
//        //    //(ES states = default(ES), ET transitions = default(ET))
//        //{
//        //    var sm = DefaultModel;
//        //    return sm.CreateState(owner);

//        //}
//    }
//}
