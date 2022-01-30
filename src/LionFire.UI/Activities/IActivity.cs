#nullable enable

using LionFire.Results;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    public interface IActivity : IKeyed<Guid>
    {
        ActivityStatus? Status { get; }
        bool IsCompleted { get; }

        string? Name { get; set; }

        Task? Task { get; }
        string? Description { get; }

        //TaskAwaiter GetAwaiter();
        
        void OnCompleted(Action continuation);
        void GetResult(); // TResult can also be void
    }

}