
namespace LionFire.Mvvm;

[Flags]
public enum SaveMode
{
    Unspecified = 0,
    OnBlur = 1 << 0,
    OnChange= 1 << 1,
    ManualPerProperty = 1 << 2,
    ManualPerObject = 1 << 3,
}
