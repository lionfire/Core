namespace LionFire.Metadata;

[Flags]
public enum RelevanceAspect
{
    Unspecified = 0,
    Read = 1 << 0,
    Write = 1 << 2,
    ReadWrite = Read | Write,
}
