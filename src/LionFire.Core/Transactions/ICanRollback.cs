using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Transactions
{
    //public interface ITraceableOperation
    //{
    //}
    //public interface IReversibleOperation
    //{

    //}
    //public interface ICanRollback
    //{
    // reason can be: a string with a message.  An exception.  A ValidationContext.  An IExceptionInformation (pending)
    //void Rollback(object reason);
    //}

    //public interface IExceptionInformation  // MOVE and REVIEW  - TODO FUTURE
    //{
    //    // TODO: copy some of the structure of an Exception?
    //    string Message { get; }
    //    //Dictionary<string,object> details
    //}

    //public interface IJournal<TTransition>
    //{
    //    // Undo stack? (or tree for full journal?)
    //}


//    namespace LionFire.StateMachines.Class
//    {
//        public interface IRemembersLastStateTransition
//        {
//            //StateChange LastStateChange { get; set; }
//        }
//        // Basis for undo/redo?
//        public interface IRemembersStateTransitions<TState, TTransition, TOwner> : IRemembersLastStateTransition
//        {
//            Queue<StateChange<TState, TTransition, TOwner>> History { get; }
//        }
//    }
}
