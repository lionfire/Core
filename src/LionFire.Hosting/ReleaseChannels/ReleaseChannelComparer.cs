#nullable enable

using System.Collections.Generic;

namespace LionFire.Deployment
{
    public class ReleaseChannelComparer : IComparer<string?>
    {
        public int Compare(string? x, string? y)
        {
            if (DefaultReleaseChannels.Dictionary.TryGetValue(x, out var xRC) &&
            DefaultReleaseChannels.Dictionary.TryGetValue(y, out var yRC)) {
                return xRC.precedence == yRC.precedence ? 0 : (xRC.precedence > yRC.precedence) ? 1 : -1;
            }
            return 0;
        }
    }
}