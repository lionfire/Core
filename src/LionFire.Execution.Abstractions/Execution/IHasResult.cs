#if UNUSED // ?
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Execution
{
    public interface IHasResult
    {
        IResult Result { get; }
        event Action<IResult> ResultChanged;
    }

    public interface IResult
    {
        bool IsSuccess { get; }
        bool IsFaulted { get; }
        bool IsCanceled { get; }
        Exception Exception { get; }

        object Data { get; }
    }
    public class ExecutionResult : IResult
    {
        public bool IsSuccess { get; set; }
        public bool IsFaulted { get; set; }
        public bool IsCanceled { get; set; }
        public Exception Exception { get; set; }
        public object Data { get; set; }
    }
}
#endif