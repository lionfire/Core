namespace LionFire.Mvvm.ObjectInspection;

public class ObservableVM : MemberVM//<PropertyInfo>
{
    public override MemberKind MemberKind => MemberKind.Event;

    // ENH - get creative with UI for this
}
