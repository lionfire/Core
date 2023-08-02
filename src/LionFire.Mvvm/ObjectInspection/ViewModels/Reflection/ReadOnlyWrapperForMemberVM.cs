namespace LionFire.Mvvm.ObjectInspection;

public class ReadOnlyWrapperForMemberVM // FUTURE ENH
{
    public MemberVM MemberVM { get; }
    public ReadOnlyWrapperForMemberVM(MemberVM memberVM)
    {
        MemberVM = memberVM;
    }

    //[Reactive]
    //public abstract bool ReadOnly { get; set;} 
}
