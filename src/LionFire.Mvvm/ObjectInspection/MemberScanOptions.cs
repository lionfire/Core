using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public class MemberScanOptions
{
    public bool Enabled { get; set; } = true;

    #region BindingFlags

    public BindingFlags BindingFlags { get; set; } = BindingFlags.Instance | BindingFlags.Public;

    #region Derived

    public bool Public
    {
        get => BindingFlags.HasFlag(BindingFlags.Public);
        set { if (value) BindingFlags |= BindingFlags.Public; else BindingFlags &= ~BindingFlags.Public; }
    }
    public bool Private { get => BindingFlags.HasFlag(BindingFlags.NonPublic); set { if (value) BindingFlags |= BindingFlags.NonPublic; else BindingFlags &= ~BindingFlags.NonPublic; } }
    public bool Static { get => BindingFlags.HasFlag(BindingFlags.Static); set { if (value) BindingFlags |= BindingFlags.Static; else BindingFlags &= ~BindingFlags.Static; } }
    public bool Instance { get => BindingFlags.HasFlag(BindingFlags.Instance); set { if (value) BindingFlags |= BindingFlags.Instance; else BindingFlags &= ~BindingFlags.Instance; } }

    #endregion

    #endregion
}
