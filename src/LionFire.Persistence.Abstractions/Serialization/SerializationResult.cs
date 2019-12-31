using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LionFire.Serialization
{
    public enum SerializationResultKind
    {
        Unspecified = 0,
        Success = 1 << 0,
        SyntaxError = 1 << 1,
        TypeError = 1 << 2,
        Unrecognized = 1 << 3,
        NotSupported = 1 << 4,
    }

    public class SerializationResult
    {
        public static readonly SerializationResult NotSupported = new SerializationResult { Flags = SerializationResultKind.NotSupported };
        public static readonly SerializationResult Success = new SerializationResult { Flags = SerializationResultKind.Success };

        public bool IsSuccess => Flags == SerializationResultKind.Success;

        public SerializationResultKind Flags { get; set; }
        public string Message { get; set; }
        public int ErrorOffset { get; set; }
        public int ErrorLength { get; set; }
        public int ErrorLineLocation { get; set; }
        public int ErrorLineNumber { get; set; }

        public IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> AggregateResults { get; set; }
    }

    public class SerializationAwaiter : INotifyCompletion
    {
        public void OnCompleted(Action continuation) => throw new NotImplementedException();
        public bool IsCompleted => true;
    }

    //public class MyAwaiter : INotifyCompletion
    //{
    //    private readonly MyAwaitable awaitable;
    //    private int result;

    //    public MyAwaiter(MyAwaitable awaitable)
    //    {
    //        this.awaitable = awaitable;
    //        if (IsCompleted)
    //            SetResult();

    //    }
    //    public bool IsCompleted => awaitable.IsFinished;

    //    public int GetResult()
    //    {
    //        if (!IsCompleted)
    //        {
    //            var wait = new SpinWait();
    //            while (!IsCompleted)
    //                wait.SpinOnce();
    //        }
    //        return result;
    //    }

    //    public void OnCompleted(Action continuation)
    //    {
    //        if (IsCompleted)
    //        {
    //            continuation();
    //            return;
    //        }
    //        var capturedContext = SynchronizationContext.Current;
    //        awaitable.Finished += () =>
    //        {
    //            SetResult();
    //            if (capturedContext != null)
    //                capturedContext.Post(_ => continuation(), null);
    //            else
    //                continuation();
    //        };
    //    }

    //    private void SetResult()
    //    {
    //        result = new Random().Next();
    //    }
    //}


    public class DeserializationResult<T> : SerializationResult
    {
        public new static readonly DeserializationResult<T> NotSupported = new DeserializationResult<T> { Flags = SerializationResultKind.NotSupported };
        public new static readonly DeserializationResult<T> Success = new DeserializationResult<T> { Flags = SerializationResultKind.Success };

        public DeserializationResult() { }
        public DeserializationResult(T successResultObject)
        {
            Object = successResultObject;
            Flags = SerializationResultKind.Success;
        }

        //public SerializationAwaiter GetAwaiter() => new SerializationAwaiter();

        public T Object { get; set; }

        public static implicit operator DeserializationResult<T>(T obj)
        {
            return new DeserializationResult<T>
            {
                Flags = SerializationResultKind.Success,
                Object = obj
            };
        }
    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
