#nullable enable
using LionFire.FlexObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Diagnostics
{
    using LionFire.Activities;

    public class ProcessActivityOptions : ActivityOptions
    {
        
        // TODO: Split this up into options classes for each Output class, such as CombinedProcessOutput
        public int CombinedOutputLines { get; set; }
        //public int ErrorLines { get; set; }
        //public int OutputLines { get; set; }

        public static readonly ProcessActivityOptions Default = new()
        {
            CombinedOutputLines = -1,
        };        
    }
}

namespace LionFire.Activities
{
    using LionFire.Diagnostics;

    public class ActivityOptions
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Foreground { get; set; }
        
    }

    public static class ProcessActivityOutputExtensions
    {
        //public static async Task<IEnumerable<string>?> WaitForOutput(this IActivity activity)
        //{
        //    await activity.ConfigureAwait(false);

        //    return activity.Query<CombinedProcessOutput>()?.CombinedOutputLines;
        //}
        public static IEnumerable<string>? Lines(this IActivity activity)
        {
            return activity.Query<CombinedProcessOutput>()?.CombinedOutputLines;
        }
    }
}
