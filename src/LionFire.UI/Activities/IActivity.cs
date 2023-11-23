#nullable enable

using LionFire.Results;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CircularBuffer;
using LionFire.FlexObjects;

namespace LionFire.Activities
{

    public interface IActivity<TResult> : IActivityBase, IActivity
    {
        new Task<TResult>? Task { get; }

        new IActivity<TResult> ConfigureAwait(bool arg1);

        new TaskAwaiter<TResult> GetAwaiter();


        //void OnCompleted(Action continuation);
        //void GetResult(); // TResult can also be void
    }

    public interface IActivity : IActivityBase
    {
        Task? Task { get; }
        TaskAwaiter GetAwaiter();
        IActivity ConfigureAwait(bool arg1);
    }

    public interface IActivityBase : IKeyed<Guid>, IFlex
    {
        ActivityOptions? Options { get; }
        ActivityStatus? Status { get; }
        bool IsCompleted { get; }

        string? Name { get; set; }

        
        string? Description { get; }
        
        

        void OnCompleted(Action continuation);
        void GetResult(); // TResult can also be void
    }

}