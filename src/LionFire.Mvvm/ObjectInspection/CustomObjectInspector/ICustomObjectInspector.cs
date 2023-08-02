namespace LionFire.Mvvm.ObjectInspection;

public interface ICustomObjectInspector
{
    /// <summary>
    /// The primary object from which the MemberVMs are derived. (Informational. Also see MemberVMs[].Source)
    /// </summary>
    object Source { get; }
    List<IMemberVM> MemberVMs { get; }
}
