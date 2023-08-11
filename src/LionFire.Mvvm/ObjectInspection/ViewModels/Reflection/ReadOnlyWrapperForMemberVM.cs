namespace LionFire.Mvvm.ObjectInspection;

public class ReadOnlyWrapperForMemberVM // FUTURE ENH
{
    public InspectorNode MemberVM { get; }
    public ReadOnlyWrapperForMemberVM(InspectorNode memberVM)
    {
        MemberVM = memberVM;
    }

    //[Reactive]
    //public abstract bool ReadOnly { get; set;} 
}
