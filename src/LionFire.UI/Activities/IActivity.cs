#nullable enable

using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    public interface IActivity : IKeyed<Guid>
    {
        ActivityStatus? Status { get; }
        bool Finished { get; }

        string? Name { get; set; }

        Task? Task { get; }
        string? Description { get; }

    }

}