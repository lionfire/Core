#nullable enable

using System.Collections.Generic;

namespace LionFire.Deployment
{
    public class ReleaseChannelComparer : IComparer<string?>
    {
        public int Compare(string? x, string? y)
        {
            if (ReleaseChannels.Dictionary.TryGetValue(x, out var xRC) &&
            ReleaseChannels.Dictionary.TryGetValue(y, out var yRC)) {
                return xRC.Precedence == yRC.Precedence ? 0 : (xRC.Precedence > yRC.Precedence) ? 1 : -1;
            }
            return 0;
        }
    }
}