using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public interface ISyncConflict
    {
        bool IsResolved { get; }

        string TheirsUrl { get; }
        string MineUrl { get; }

        Task TakeTheirs();
        Task TakeMine();

        IEnumerable<ISyncDifference> Differences { get; }

    }



}
