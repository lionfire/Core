#nullable enable


using LionFire;

namespace LionFire.Persisters.Expanders;

public class ExpansionMountOptions
{
    public char PrefixCharacter { get; set; } = ']';
    public char SuffixCharacter { get; set; } = '[';

    public int ReadCount { get; } = 1;
    public int ReadWriteCount { get; } = 2;
    public int WriteCount { get; } = 3;

    public TimeSpan? TimeBetweenScans { get; set; } = TimeSpan.FromMinutes(5);
}

