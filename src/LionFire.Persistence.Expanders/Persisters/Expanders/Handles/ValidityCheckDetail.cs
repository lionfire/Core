namespace LionFire.Persisters.Expanders;

public enum ValidityCheckDetail
{
    Unspecified = 0,
    Quick = 1 << 0,
    Full = 1 << 8,
}
